using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Further.Operation.Operations
{
    public class OperationResult
    {
        public  bool IsFailed { get; set; }

        public  bool IsSuccess { get; set; }

        public  List<OperationReason> Reasons { get; set; } = new();

        public  List<OperationReason> Errors { get; set; } = new();

        public  List<OperationReason> Successes { get; set; } = new();

        public OperationResult()
        {
        }

        public OperationResult(Result result)
        {
            IsFailed = result.IsFailed;
            IsSuccess = result.IsSuccess;

            Reasons = result.Reasons.Select(r => new OperationReason
            {
                Message = r.Message,
                Metadata = r.Metadata
            }).ToList();

            Errors = result.Errors.Select(r => new OperationReason
            {
                Message = r.Message,
                Metadata = r.Metadata
            }).ToList();

            Successes = result.Successes.Select(r => new OperationReason
            {
                Message = r.Message,
                Metadata = r.Metadata
            }).ToList();
        }

        public Result CopyToResult()
        {
            var result = new Result();

            //foreach (var reason in Reasons)
            //{
            //    result.WithReason(reason);
            //}

            foreach (var error in Errors)
            {
                var reason = new Error(error.Message);

                reason = (Error)(error.CopyMetadataReason(reason));

                result.WithError(reason);
            }

            foreach (var success in Successes)
            {
                var reason = new Success(success.Message);

                reason = (Success)(success.CopyMetadataReason(reason));

                result.WithSuccess(reason);

            }

            return result;
        }
    }

    public class OperationReason: IReason
    {
        public string Message { get; set; } = null!;

        public Dictionary<string, object> Metadata { get; set; } = new();

        public IReason CopyMetadataReason(IReason reason)
        {
            foreach (var metadata in Metadata)
            {
                reason.Metadata.Add(metadata.Key, metadata.Value);
            }

            return reason;
        }
    }
}
