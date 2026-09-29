using System;
using System.Reflection.Emit;
using Moq;
using Moq.AutoMock;
using NES_Emulator;

namespace EmulatorTests;

public interface IDB
{
    int Dobrar(int n);
}

public class DB : IDB
{
    public int Dobrar(int n)
    {
        return n * 2;
    }
}

public class Calculadora 
{
    private readonly IDB _db;
    public Calculadora(IDB db)
    {
        _db = db;
    }

    public int Somar(int n1, int n2)
    {
        var r = n1  + n2;

        return _db.Dobrar(r);
    }

    public int Somar2(int n1, int n2)
    {
        if (n1 > 10)
        {
            return SomaQuandoForMaior10(n1, n2);
        }
        else
        {
            return SomaQuandoForMenor10(n1, n2);
        }
    }

    public int SomaQuandoForMaior10(int n1, int n2)
    {
         return n1 + n2 + 10;
    }

    public int SomaQuandoForMenor10(int n1, int n2)
    {
         return n1 + n2 + 1;
    }



}

public class CalculadoraTest
{

    private readonly Calculadora calc;
    private readonly AutoMocker mocker;
    
    public CalculadoraTest()
    {
        mocker = new AutoMocker();
        calc = mocker.CreateInstance<Calculadora>();
    }

    [Fact]
    public void Somar()
    {
        mocker.GetMock<IDB>()
            .Setup(x => x.Dobrar(3))
            .Returns(6);

        var result = calc.Somar(1, 2);
        
        Assert.Equal(6, result);
    }

    
    [Fact]
    public void Somar_Verificar_Se_Dobra_Foi_Chamada()
    {
        calc.Somar(1, 2);
        
        mocker.GetMock<IDB>()
            .Verify(x => x.Dobrar(It.IsAny<int>()),Times.Once);
    }
    
    /*
    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(0, 2, 2)]
    public void SomarComTeoria(int n1, int n2, int resultado)
    {
        var result = calc.Somar(n1, n2);

        Assert.Equal(resultado, result);
    }
    */
}