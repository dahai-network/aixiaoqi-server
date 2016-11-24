using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Model
{
    public class MessageConfiguration : EntityTypeConfiguration<UT_Message>
    {
        public MessageConfiguration()
        {
            //消息图片一对多
            this.HasMany(t => t.UT_MessagePhoto).WithRequired(t => t.UT_Message).HasForeignKey(t => t.MessageId);
            //评论一对多
            this.HasMany(t => t.UT_MessageComment).WithRequired(t => t.UT_Message).HasForeignKey(t => t.MessageId);
            //点赞一对多
            this.HasMany(t => t.UT_MessageLike).WithRequired(t => t.UT_Message).HasForeignKey(t => t.MessageId);

            this.Property(t => t.Ip).HasMaxLength(20).IsRequired();

            this.Property(t => t.Country).HasMaxLength(20).IsRequired();

            this.Property(t => t.Location).HasMaxLength(50).IsRequired();

            this.Property(t => t.Content).HasColumnType("text").IsRequired();
        }
    }
}
