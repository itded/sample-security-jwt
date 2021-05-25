using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JwtAuthServer.Authentication.Models
{
    public abstract class ResponseBase
    {
        public IReadOnlyList<ResponseError> Errors { get; }

        public bool Succeeded => Errors == null || !Errors.Any();

        protected ResponseBase()
        {
            Errors = Array.Empty<ResponseError>();
        }

        protected ResponseBase(params ResponseError[] errors)
        {
            Errors = new ReadOnlyCollection<ResponseError>(errors ?? Array.Empty<ResponseError>());
        }
    }
}
