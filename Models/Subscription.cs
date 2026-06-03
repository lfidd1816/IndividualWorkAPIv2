using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndividualWorkAPI.Models;

[Index(nameof(subscriberId), nameof(channelId), IsUnique = true)]
public class Subscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int subscriptionId { get; set; }
    public DateTime createdAt { get; set; }
   
    [Required]
    [ForeignKey("subscriberId")]
    public int subscriberId { get; set; }
    public User subscriber { get; set; }
    
    [Required]
    [ForeignKey("channelId")]
    public int channelId { get; set; }
    public User channel { get; set; }
    
   
}

public class UserFollowConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(e => new { e.subscriberId, e.channelId });

        builder.HasCheckConstraint(
            "CK_UserFollow_IdsNotEqual", 
            "subscriberId <> channelId"
        );

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.subscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.channelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}