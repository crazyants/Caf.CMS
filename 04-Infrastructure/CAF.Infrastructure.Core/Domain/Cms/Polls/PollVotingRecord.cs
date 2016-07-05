

using CAF.Infrastructure.Core.Domain.Users;
namespace CAF.Infrastructure.Core.Domain.Cms.Polls
{
    /// <summary>
    /// Represents a poll voting record
    /// </summary>
    public partial class PollVotingRecord : UserContent
    {
        /// <summary>
        /// Gets or sets the poll answer identifier
        /// </summary>
        public int PollAnswerId { get; set; }

        /// <summary>
        /// Gets or sets the poll answer
        /// </summary>
        public virtual PollAnswer PollAnswer { get; set; }
    }
}