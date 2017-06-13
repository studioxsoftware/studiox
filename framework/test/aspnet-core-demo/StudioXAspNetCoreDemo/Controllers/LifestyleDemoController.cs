using System;
using StudioX.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace StudioXAspNetCoreDemo.Controllers
{
    public class LifestyleDemoController : StudioXController
    {
        private readonly MyTransientClass1 myTransientClass1;
        private readonly MyTransientClass2 myTransientClass2;

        public LifestyleDemoController(MyTransientClass1 myTransientClass1, MyTransientClass2 myTransientClass2)
        {
            this.myTransientClass1 = myTransientClass1;
            this.myTransientClass2 = myTransientClass2;
        }

        public ActionResult Test1()
        {
            return new ContentResult()
            {
                ContentType = "text/plain",
                Content = $"myTransientClass1.ScopedClass.Id = {myTransientClass1.ScopedClass.Id}; myTransientClass2.ScopedClass.Id = {myTransientClass2.ScopedClass.Id}"
            };
        }
    }

    public class MyTransientClass1
    {
        public MyScopedClass ScopedClass { get; }

        public MyTransientClass1(MyScopedClass scopedClass)
        {
            ScopedClass = scopedClass;
        }
    }

    public class MyTransientClass2
    {
        public MyScopedClass ScopedClass { get; }

        public MyTransientClass2(MyScopedClass scopedClass)
        {
            ScopedClass = scopedClass;
        }
    }

    public class MyScopedClass
    {
        public string Id { get; } = Guid.NewGuid().ToString("N");
    }
}
