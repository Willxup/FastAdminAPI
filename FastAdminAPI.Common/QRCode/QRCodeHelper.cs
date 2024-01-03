﻿using QRCoder;

namespace FastAdminAPI.Common.QRCode
{
    public static class QRCodeHelper
    {
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="pixel">像素大小</param>
        /// <returns></returns>
        public static byte[] GetQRCode(string content, int pixel)
        {
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
            PngByteQRCode qRCode = new(qrCodeData);
            return qRCode.GetGraphic(pixel);
        }
    }
}
