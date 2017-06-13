using StudioX.Domain.Entities;
using StudioX.Events.Bus.Entities;
using Xunit;

namespace StudioX.Tests.Experimental
{
    public class GenericMethodCallTests
    {
        [Fact]
        public void TestMethodBaseEventBaseArg()
        {
            MethodBaseEventBaseArg(new EntityEventData<Person>(new Person())); //TODO: <Student>
            MethodBaseEventBaseArg(new EntityEventData<Person>(new Student())); //TODO: <Student>
            MethodBaseEventBaseArg(new EntityUpdatedEventData<Person>(new Person())); //TODO: <Student>
            MethodBaseEventBaseArg(new EntityUpdatedEventData<Person>(new Student())); //TODO: <Student>
        }

        public void MethodBaseEventBaseArg(EntityEventData<Person> data)
        {

        }

        [Fact]
        public void TestMethodBaseEventDerivedArg()
        {
            MethodBaseEventDerivedArg(new EntityEventData<Student>(new Student()));
            MethodBaseEventDerivedArg(new EntityUpdatedEventData<Student>(new Student()));
        }

        public void MethodBaseEventDerivedArg(EntityEventData<Student> data)
        {

        }

        [Fact]
        public void TestMethodDerivedEventBaseArg()
        {
            MethodDerivedEventBaseArg(new EntityUpdatedEventData<Person>(new Person()));
            MethodDerivedEventBaseArg(new EntityUpdatedEventData<Person>(new Student()));
        }

        public void MethodDerivedEventBaseArg(EntityUpdatedEventData<Person> data)
        {

        }

        [Fact]
        public void TestMethodDerivedEventDerivedArg()
        {
            MethodDerivedEventDerivedArg(new EntityUpdatedEventData<Student>(new Student()));
        }

        public void MethodDerivedEventDerivedArg(EntityUpdatedEventData<Student> data)
        {

        }

        public class Person : Entity
        {

        }

        public class Student : Person
        {

        }
    }
}