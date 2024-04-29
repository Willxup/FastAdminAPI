using QRCoder;

namespace FastAdminAPI.Common.QRCode
{
    public static class QRCodeTool
    {
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns>二维码图片</returns>
        public static byte[] GetQRCode(string content, int pixel)
        {
            QRCodeData qrCodeData = QRCodeGenerator.GenerateQrCode(content, QRCodeGenerator.ECCLevel.M);
            PngByteQRCode qRCode = new(qrCodeData);
            return qRCode.GetGraphic(pixel);
        }
    }
}
