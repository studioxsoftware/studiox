using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace StudioX.Domain.Uow
{
    /// <summary>
    /// This handle is used for innet unit of work scopes.
    /// A inner unit of work scope actually uses outer unit of work scope
    /// and has no effect on <see cref="IUnitOfWorkCompleteHandle.Complete"/> call.
    /// But if it's not called, an exception is thrown at end of the UOW to rollback the UOW.
    /// </summary>
    internal class InnerUnitOfWorkCompleteHandle : IUnitOfWorkCompleteHandle
    {
        public const string DidNotCallCompleteMethodExceptionMessage = "Did not call Complete method of a unit of work.";

        private volatile bool isCompleteCalled;
        private volatile bool isDisposed;

        public void Complete()
        {
            isCompleteCalled = true;
        }

        public Task CompleteAsync()
        {
            isCompleteCalled = true;
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            if (!isCompleteCalled)
            {
                if (HasException())
                {
                    return;
                }

                throw new StudioXException(DidNotCallCompleteMethodExceptionMessage);
            }
        }
        
        private static bool HasException()
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}