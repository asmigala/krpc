using System;
using KRPC.Continuations;
using KRPC.Server;
using KRPC.Service.Scanner;
using KRPC.Schema.KRPC;

namespace KRPC.Service
{
    /// <summary>
    /// A continuation that runs a client request.
    /// </summary>
    class RequestContinuation : Continuation<Response>
    {
        public IClient Client { get; private set; }

        readonly Request request;
        readonly ProcedureSignature procedure;
        readonly Exception exception;
        readonly IContinuation continuation;

        public RequestContinuation (IClient client, Request request)
        {
            Client = client;
            this.request = request;
            try {
                procedure = Services.Instance.GetProcedureSignature (request);
            } catch (Exception e) {
                exception = e;
            }
        }

        RequestContinuation (IClient client, Request request, ProcedureSignature procedure, IContinuation continuation)
        {
            Client = client;
            this.request = request;
            this.procedure = procedure;
            this.continuation = continuation;
        }

        public override Response Run ()
        {
            if (exception != null)
                throw exception;
            try {
                if (continuation == null)
                    return Services.Instance.HandleRequest (procedure, request);
                else
                    return Services.Instance.HandleRequest (procedure, continuation);
            } catch (YieldException e) {
                throw new YieldException (new RequestContinuation (Client, request, procedure, e.Continuation));
            }
        }
    };
}
