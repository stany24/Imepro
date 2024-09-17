using System.Text;

namespace ClassLibrary6.ReliableMulticast
{
    /// <summary>
    /// Class representing a UDP multicast message.
    /// </summary>
    public class ReliableMulticastMessage
    {
        #region Variables

        public byte[] Data { get; }
        public readonly int ImageNumber;
        public readonly int PartNumber;
        public readonly int TotalPartNumber;
        private const string Separator = "[";

        #endregion

        #region Constructor

        public ReliableMulticastMessage(string customString)
        {
            string[] split = customString.Split(Separator[0]);
            ImageNumber = Convert.ToInt32(split[0]);
            PartNumber = Convert.ToInt32(split[1]);
            TotalPartNumber = Convert.ToInt32(split[2]);
            int headerLength = 3 + split[0].Length + split[1].Length + split[2].Length;
            Data = new byte[customString.Length - headerLength];
            Array.Copy(Encoding.Default.GetBytes(customString), headerLength, Data, 0, customString.Length - headerLength);
        }

        public ReliableMulticastMessage(byte[] data, int imageNumber, int partnumber, int totalPartNumber)
        {
            Data = data;
            ImageNumber = imageNumber;
            PartNumber = partnumber;
            TotalPartNumber = totalPartNumber;
        }

        #endregion

        public string ToCustomString()
        {
            return ImageNumber + Separator + PartNumber + Separator + TotalPartNumber + Separator + Encoding.Default.GetString(Data);
        }
    }
}
