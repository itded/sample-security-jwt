using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

        public virtual string PrintErrors()
        {
            var errorCount = Errors.Count;
            if (errorCount == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            for (var i = 0; i < errorCount; i++)
            {
                var error = Errors[i];
                builder.Append(error.Code).Append(": ").Append(error.Description);

                if (i != errorCount - 1)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }
    }
}
