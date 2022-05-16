namespace Kentico.Xperience.AmazonPersonalize
{
    /// <summary>
    /// Encapsulates event tracker for Amazon Personalize.
    /// </summary>
    public interface IEventTrackerService 
    {
        /// <summary>
        /// Id of active event tracker needed for sending events to Amazon Personalize.
        /// </summary>
        string TrackingId { get; }
    }
}