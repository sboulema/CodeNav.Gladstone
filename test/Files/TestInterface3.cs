namespace CodeNav.Test.Files
{
    interface ITestInterface3
    {
        int InterfaceMethod1();
    }

    interface ITestInterface4
    {
        int InterfaceMethod2();
    }

    interface ITestInterface5 : ITestInterface4
    {
        int InterfaceMethod3();
    }

    class BaseClass : ITestInterface5
    {
        public int InterfaceMethod2()
        {
            throw new NotImplementedException();
        }

        public int InterfaceMethod3()
        {
            throw new NotImplementedException();
        }
    }

    class ImplementingClass3 : BaseClass, ITestInterface3
    {
        public int InterfaceMethod1()
        {
            throw new NotImplementedException();
        }
    }
}
