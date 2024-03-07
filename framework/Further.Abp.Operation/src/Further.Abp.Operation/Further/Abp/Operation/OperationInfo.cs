using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Further.Abp.Operation
{
    public class OperationInfo
    {
        public Guid Id { get; private set; }
        public string? OperationId { get; set; }

        public string? OperationName { get; set; }

        public Result Result { get; } = Result.Ok();

        public bool IsSuccess => this.Result.IsSuccess;

        public List<OperationOwnerInfo> Owners { get; } = new();

        public int ExecutionDuration { get; set; } = 0;

        [JsonConstructor]
        public OperationInfo(Guid id)
        {
            this.Id = id;
        }

        public OperationInfo(Guid id, string? operationId, string? operationName,Result result, List<OperationOwnerInfo> owners,int executionDuration)
        {
            this.Id = id;
            this.OperationId = operationId;
            this.OperationName = operationName;
            this.Result = result;
            this.Owners = owners;
            this.ExecutionDuration = executionDuration;
        }
    }
}
