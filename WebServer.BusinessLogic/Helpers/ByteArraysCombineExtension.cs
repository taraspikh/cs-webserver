namespace WebServer.BusinessLogic.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    public static class ByteArraysCombineExtension
    {
        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
