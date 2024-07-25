namespace RateLimiter.Enums
{
    /// <summary>
    /// List of services that need rate limiting
    /// </summary>
    public enum ServiceType
    {
        GenerateLabelService = 0,
        LabelAdjustmentService = 1,
        RefundLabelService = 2,
        EmailService = 3
    }
}