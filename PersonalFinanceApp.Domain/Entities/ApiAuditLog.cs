using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Entities;

public class ApiAuditLog
{
    public Guid Id {get;set;}
    public DateTime Timestamp {get;set;}

    public Guid? UserId {get;set;}
    public Guid? TenantId {get;set;}


    public string HttpMethod { get; private set; } = string.Empty;
    public string RequestPath { get; private set; } = string.Empty;
    public string? ControllerName { get; private set; }
    public string? ActionName { get; private set; }
    public int StatusCode { get; private set; }
    public long DurationMs { get; private set; }
    public string CorrelationId { get; private set; } = string.Empty;
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public bool IsSuccess { get; private set; }


    private ApiAuditLog(){}

    public ApiAuditLog(
        Guid? userId,
        Guid? tenantId,
        string httpMethod,
        string requestPath,
        string? controllerName,
        string? actionName,
        int statusCode,
        long durationMs,
        string correlationId,
        string? ipAddress,
        string? userAgent)
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        UserId = userId;
        TenantId = tenantId;
        HttpMethod = httpMethod;
        RequestPath = requestPath;
        ControllerName = controllerName;
        ActionName = actionName;
        StatusCode = statusCode;
        DurationMs = durationMs;
        CorrelationId = correlationId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        IsSuccess = statusCode < 400;
    }


}
