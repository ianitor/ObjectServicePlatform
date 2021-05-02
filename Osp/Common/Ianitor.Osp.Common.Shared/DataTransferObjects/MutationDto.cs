namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class MutationDto<TItemType> : MutationDto
    {
        public TItemType Item { get; set; }
    }

    public class MutationDto
    {
        public OspObjectId RtId { get; set; }
    }
}