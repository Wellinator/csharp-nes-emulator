#!/usr/bin/env python3
"""
Compara o log de execução do emulador com o log de referência do nestest
(nestest.log, gerado pelo Nintendulator) e mostra onde a execução diverge.

Formato esperado de cada linha (o texto de disassembly no meio é ignorado):
    C000  4C F5 C5  JMP $C5F5        A:00 X:00 Y:00 P:24 SP:FD PPU:  0, 21 CYC:7

Uso:
    # comparar dois arquivos
    python tools/nestest_compare.py emu.log

    # rodar o emulador e comparar a saída em tempo real (para na 1ª divergência)
    python tools/nestest_compare.py --run "dotnet run --project Emulator"

    # ignorar ciclos enquanto a contagem ainda não está implementada
    python tools/nestest_compare.py emu.log --no-cycles
"""
from __future__ import annotations

import argparse
import os
import re
import subprocess
import sys
from collections import deque
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable, Iterator

DEFAULT_GOLDEN = Path(r"D:\CSharp\NES Test Roms\other\nestest.log")

LINE_RE = re.compile(
    r"^(?P<pc>[0-9A-Fa-f]{4})\s+"
    r"(?P<bytes>(?:[0-9A-Fa-f]{2}\s)+)"
    r".*?"
    r"A:(?P<a>[0-9A-Fa-f]{2})\s+"
    r"X:(?P<x>[0-9A-Fa-f]{2})\s+"
    r"Y:(?P<y>[0-9A-Fa-f]{2})\s+"
    r"P:(?P<p>[0-9A-Fa-f]{2})\s+"
    r"SP:(?P<sp>[0-9A-Fa-f]{2})"
    r"(?:.*?CYC:\s*(?P<cyc>\d+))?"
)

FLAG_NAMES = "NV-BDIZC"  # bit 7 .. bit 0

# O console do Windows usa cp1252 por padrão, que não tem ✓/✗
sys.stdout.reconfigure(encoding="utf-8", errors="replace")

# Cores ANSI (o Windows 10+ aceita depois do os.system(""))
if sys.stdout.isatty() and not os.environ.get("NO_COLOR"):
    os.system("")
    RED, GREEN, YELLOW, DIM, BOLD, RESET = (
        "\033[31m", "\033[32m", "\033[33m", "\033[2m", "\033[1m", "\033[0m")
else:
    RED = GREEN = YELLOW = DIM = BOLD = RESET = ""


@dataclass
class State:
    lineno: int
    raw: str
    pc: int
    opbytes: tuple[int, ...]
    a: int
    x: int
    y: int
    p: int
    sp: int
    cyc: int | None

    @property
    def opcode(self) -> int:
        return self.opbytes[0]


def parse(lines: Iterable[str]) -> Iterator[State]:
    """Converte linhas de log em States, ignorando linhas que não casam o formato."""
    for n, line in enumerate(lines, 1):
        line = line.rstrip("\r\n")
        m = LINE_RE.match(line)
        if not m:
            continue
        yield State(
            lineno=n,
            raw=line,
            pc=int(m["pc"], 16),
            opbytes=tuple(int(b, 16) for b in m["bytes"].split()),
            a=int(m["a"], 16),
            x=int(m["x"], 16),
            y=int(m["y"], 16),
            p=int(m["p"], 16),
            sp=int(m["sp"], 16),
            cyc=int(m["cyc"]) if m["cyc"] else None,
        )


def flags_str(p: int) -> str:
    return "".join(FLAG_NAMES[i] if p & (0x80 >> i) else "." for i in range(8))


def diff_flags(exp: int, got: int, mask: int) -> str:
    out = []
    for i in range(8):
        bit = 0x80 >> i
        if not mask & bit:
            continue
        if (exp ^ got) & bit:
            out.append(f"{FLAG_NAMES[i]} esperado={int(bool(exp & bit))} obtido={int(bool(got & bit))}")
    return ", ".join(out)


def compare(exp: State, got: State, args) -> list[str]:
    """Retorna a lista de campos divergentes (vazia se a linha bate)."""
    errors = []
    if exp.pc != got.pc:
        errors.append(f"PC   esperado={exp.pc:04X} obtido={got.pc:04X}")
    if exp.opbytes != got.opbytes:
        e = " ".join(f"{b:02X}" for b in exp.opbytes)
        g = " ".join(f"{b:02X}" for b in got.opbytes)
        errors.append(f"BYTES esperado=[{e}] obtido=[{g}]")
    for reg in ("a", "x", "y", "sp"):
        ev, gv = getattr(exp, reg), getattr(got, reg)
        if ev != gv:
            errors.append(f"{reg.upper():<4} esperado={ev:02X} obtido={gv:02X}")
    mask = args.p_mask
    if (exp.p & mask) != (got.p & mask):
        errors.append(
            f"P    esperado={exp.p:02X} ({flags_str(exp.p)}) obtido={got.p:02X} ({flags_str(got.p)})"
            f" -> {diff_flags(exp.p, got.p, mask)}")
    if not args.no_cycles and exp.cyc is not None and got.cyc is not None and exp.cyc != got.cyc:
        errors.append(f"CYC  esperado={exp.cyc} obtido={got.cyc} (delta {got.cyc - exp.cyc:+d})")
    return errors


def print_divergence(exp: State, got: State, errors: list[str], history: deque, prev_exp: State | None):
    print(f"\n{RED}{BOLD}✗ Divergência na instrução #{exp.lineno} do nestest.log{RESET}")
    if history:
        print(f"{DIM}  Últimas instruções corretas:{RESET}")
        for h in history:
            print(f"{DIM}    {h}{RESET}")
    if prev_exp is not None:
        # O estado impresso é o de ANTES da instrução, então quem causou o erro
        # quase sempre é a instrução anterior.
        print(f"\n  {YELLOW}Provável culpada (instrução anterior, opcode ${prev_exp.opcode:02X}):{RESET}")
        print(f"    {prev_exp.raw}")
    print(f"\n  {GREEN}esperado:{RESET} {exp.raw}")
    print(f"  {RED}obtido:  {RESET} {got.raw}")
    print()
    for e in errors:
        print(f"    {RED}•{RESET} {e}")


def run_emulator(cmd: str) -> tuple[subprocess.Popen, Iterator[str]]:
    proc = subprocess.Popen(
        cmd, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
        text=True, encoding="utf-8", errors="replace", bufsize=1)
    return proc, iter(proc.stdout.readline, "")


def main() -> int:
    ap = argparse.ArgumentParser(description="Compara log do emulador com o nestest.log")
    src = ap.add_mutually_exclusive_group(required=True)
    src.add_argument("emu_log", nargs="?", help="log gerado pelo emulador ('-' = stdin)")
    src.add_argument("--run", metavar="CMD", help="comando que roda o emulador e imprime o log no stdout")
    ap.add_argument("--golden", type=Path, default=DEFAULT_GOLDEN, help=f"log de referência (padrão: {DEFAULT_GOLDEN})")
    ap.add_argument("--no-cycles", action="store_true", help="não compara a contagem de ciclos")
    ap.add_argument("--max-errors", type=int, default=1,
                    help="quantas divergências mostrar antes de parar (padrão 1; depois da 1ª, as seguintes costumam ser efeito cascata)")
    ap.add_argument("--context", type=int, default=5, help="quantas linhas corretas mostrar antes da divergência")
    ap.add_argument("--ignore-unused-flags", action="store_true",
                    help="ignora os bits 4 (B) e 5 (-) do P, que não existem fisicamente no registrador")
    args = ap.parse_args()
    args.p_mask = 0xCF if args.ignore_unused_flags else 0xFF

    golden = list(parse(args.golden.read_text(encoding="utf-8", errors="replace").splitlines()))

    proc = None
    if args.run:
        proc, emu_lines = run_emulator(args.run)
    elif args.emu_log == "-":
        emu_lines = sys.stdin
    else:
        emu_lines = open(args.emu_log, encoding="utf-8", errors="replace")

    history: deque[str] = deque(maxlen=args.context)
    ok = 0
    n_errors = 0
    first_bad: State | None = None
    emu_iter = parse(emu_lines)
    prev_exp = None

    try:
        for exp in golden:
            got = next(emu_iter, None)
            if got is None:
                print(f"\n{YELLOW}⚠ O emulador parou depois de {ok} instruções corretas "
                      f"(referência tem {len(golden)}).{RESET}")
                print(f"  Próxima instrução esperada: {exp.raw}")
                break
            errors = compare(exp, got, args)
            if errors:
                n_errors += 1
                first_bad = first_bad or exp
                print_divergence(exp, got, errors, history, prev_exp)
                if n_errors >= args.max_errors:
                    break
            else:
                ok += 1
            history.append(got.raw)
            prev_exp = exp
    finally:
        if proc is not None:
            proc.kill()

    total = len(golden)
    pct = 100.0 * ok / total if total else 0
    print(f"\n{BOLD}Resumo:{RESET} {ok}/{total} instruções corretas ({pct:.1f}%)")
    if first_bad is None and ok == total:
        print(f"{GREEN}{BOLD}✓ Log idêntico ao nestest.log!{RESET}")
        return 0
    if first_bad is not None:
        print(f"Primeira divergência na linha {first_bad.lineno} do log de referência (PC=${first_bad.pc:04X}).")
    return 1


if __name__ == "__main__":
    sys.exit(main())
