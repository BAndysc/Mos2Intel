using NUnit.Framework;
using X86Assembly;

namespace X86Test;

public class Tests
{
    [Test]
    public void Test1()
    {
        Assert.AreEqual("LEA R11, [rel _a]", X86.Lea(RegisterX86.R11, X86.Indirect(0, 0, 1, "_a")).ToString());
        Assert.AreEqual("MOV R11, R12", X86.Mov(RegisterX86.R11, RegisterX86.R12).ToString());
        Assert.AreEqual("MOV R11, 0x1", X86.Mov(RegisterX86.R11, 1).ToString());
        Assert.AreEqual("MOV R11, [0x1]", X86.Mov(RegisterX86.R11, X86.Indirect(0, 1)).ToString());
        Assert.AreEqual("MOV R11, [R12]", X86.Mov(RegisterX86.R11, X86.Indirect(RegisterX86.R12)).ToString());
        Assert.AreEqual("MOV R11, [2 * R12]", X86.Mov(RegisterX86.R11, X86.Indirect(RegisterX86.R12,0,2)).ToString());
    }

    [Test]
    public void TestRegisters()
    {
        Assert.AreEqual(RegisterX86.RAX, RegisterX86.AL.WithWidth(RegisterWidth.QWord));
        Assert.AreEqual(RegisterX86.RAX, RegisterX86.AX.WithWidth(RegisterWidth.QWord));
        Assert.AreEqual(RegisterX86.RAX, RegisterX86.EAX.WithWidth(RegisterWidth.QWord));
        Assert.AreEqual(RegisterX86.RAX, RegisterX86.RAX.WithWidth(RegisterWidth.QWord));
        
        Assert.AreEqual(RegisterX86.EAX, RegisterX86.AL.WithWidth(RegisterWidth.DWord));
        Assert.AreEqual(RegisterX86.EAX, RegisterX86.AX.WithWidth(RegisterWidth.DWord));
        Assert.AreEqual(RegisterX86.EAX, RegisterX86.EAX.WithWidth(RegisterWidth.DWord));
        Assert.AreEqual(RegisterX86.EAX, RegisterX86.RAX.WithWidth(RegisterWidth.DWord));
        
        Assert.AreEqual(RegisterX86.AX, RegisterX86.AL.WithWidth(RegisterWidth.Word));
        Assert.AreEqual(RegisterX86.AX, RegisterX86.AX.WithWidth(RegisterWidth.Word));
        Assert.AreEqual(RegisterX86.AX, RegisterX86.EAX.WithWidth(RegisterWidth.Word));
        Assert.AreEqual(RegisterX86.AX, RegisterX86.RAX.WithWidth(RegisterWidth.Word));
        
        Assert.AreEqual(RegisterX86.AL, RegisterX86.AL.WithWidth(RegisterWidth.Byte));
        Assert.AreEqual(RegisterX86.AL, RegisterX86.AX.WithWidth(RegisterWidth.Byte));
        Assert.AreEqual(RegisterX86.AL, RegisterX86.EAX.WithWidth(RegisterWidth.Byte));
        Assert.AreEqual(RegisterX86.AL, RegisterX86.RAX.WithWidth(RegisterWidth.Byte));
    }
}