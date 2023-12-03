using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.ModelBuilding
{
    public class ConversationEntryConfigurator : IEntityTypeConfiguration<ConversationEntry>
    {
        public void Configure(EntityTypeBuilder<ConversationEntry> builder)

        {
            builder.ToTable("ConversationEntries");
        }
    }
}