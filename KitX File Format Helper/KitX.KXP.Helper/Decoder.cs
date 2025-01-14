﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KitX.KXP.Helper
{
    public class Decoder
    {
        public Decoder(string packagePath)
        {
            PackagePath = packagePath;
        }

        public string PackagePath { get; set; }

        /// <summary>
        /// 文件地图项
        /// </summary>
        internal struct FileMapItem
        {
            /// <summary>
            /// 文件名长度
            /// </summary>
            internal long FileNamePathLength;

            /// <summary>
            /// 文件体长度
            /// </summary>
            internal long FileBodyLength;
        }

        /// <summary>
        /// 解码包体
        /// </summary>
        /// <returns>返回 LoaderStruct 的json文本</returns>
        /// <param name="releaseFolder">释放文件的路径</param>
        /// <exception cref="Exception">哈希校验错误</exception>
        public Tuple<string, string> Decode(string releaseFolder)
        {
            if (!Directory.Exists(releaseFolder))
                _ = Directory.CreateDirectory(releaseFolder);

            byte[] hash = new byte[16];     //  哈希部分

            byte[] src = File.ReadAllBytes(PackagePath);    //  读取包的全部字节

            byte[] header = new byte[16];

            for (int i = 0; i < 16; ++i)
                header[i] = src[i];

            for (int i = 0; i < 16; ++i)
                if (header[i] != Header.header[i])
                    throw new Exception("It's not a KXP Package.");

            int cursor = 16;     //  文件流指针

            for (; cursor < 32; ++cursor)       //  取出哈希部分的字节
                hash[cursor - 16] = src[cursor];

#if DEBUG
            Console.WriteLine($"Hash Code: {Encoding.UTF8.GetString(hash)}");
#endif

            byte[] fileMapLengthByte = new byte[8];     //  文件地图长度

            for (int i = 0; i < 8; ++i, ++cursor)
                fileMapLengthByte[i] = src[cursor];

            long fileMapLength = BitConverter.ToInt64(fileMapLengthByte, 0);

#if DEBUG
            Console.WriteLine($"File Map Length: {fileMapLength}");
#endif

            List<FileMapItem> FileMap = new List<FileMapItem>();        //  文件地图

            for (long i = 0; i < fileMapLength; ++i)
            {
                byte[] fileNameLength = new byte[8];    //  文件名长度
                byte[] fileBodyLength = new byte[8];    //  文件体长度

                for (int j = 0; j < 8; ++j, ++cursor)
                    fileNameLength[j] = src[cursor];

                for (int j = 0; j < 8; ++j, ++cursor)
                    fileBodyLength[j] = src[cursor];

                FileMap.Add(new FileMapItem()
                {
                    FileNamePathLength = BitConverter.ToInt64(fileNameLength, 0),
                    FileBodyLength = BitConverter.ToInt64(fileBodyLength, 0)
                });

#if DEBUG
                Console.WriteLine($"File Name Length: {BitConverter.ToInt64(fileNameLength, 0)}");
                Console.WriteLine($"File Body Length: {BitConverter.ToInt64(fileBodyLength, 0)}");
#endif
            }

            Dictionary<byte[], byte[]> Files = new Dictionary<byte[], byte[]>();    //  文件列表

            foreach (var item in FileMap)
            {
                byte[] fn = new byte[item.FileNamePathLength];
                byte[] fb = new byte[item.FileBodyLength];

                for (int i = 0; i < item.FileNamePathLength; ++i, ++cursor)
                    fn[i] = src[cursor];

                for (int i = 0; i < item.FileBodyLength; ++i, ++cursor)
                    fb[i] = src[cursor];

                File.WriteAllBytes($"{releaseFolder}\\{Encoding.UTF8.GetString(fn)}", fb);
            }

            byte[] loaderStructLengthByte = new byte[8];

            for (int i = 0; i < 8; ++i, ++cursor)
                loaderStructLengthByte[i] = src[cursor];

            long loaderStructLength = BitConverter.ToInt64(loaderStructLengthByte, 0);  //  LoaderStruct的长度

#if DEBUG
            Console.WriteLine($"Loader Struct Length: {loaderStructLength}");
#endif

            byte[] loaderStructByte = new byte[loaderStructLength];     //  Loader Struct 的 Byte 数组

            for (int i = 0; i < loaderStructLength; ++i, ++cursor)
                loaderStructByte[i] = src[cursor];

            byte[] pluginStructLengthByte = new byte[8];

            for (int i = 0; i < 8; ++i, ++cursor)
                pluginStructLengthByte[i] = src[cursor];

            long pluginStructLength = BitConverter.ToInt64(pluginStructLengthByte, 0);  //  PluginStruct的长度

#if DEBUG
            Console.WriteLine($"Plugin Struct Length: {pluginStructLength}");
#endif

            byte[] pluginStructByte = new byte[pluginStructLength];     //  Plugin Struct 的 Byte 数组

            for (int i = 0; i < pluginStructLength; ++i, ++cursor)
                pluginStructByte[i] = src[cursor];

            string loaderStruct = Encoding.UTF8.GetString(loaderStructByte);
            string pluginStruct = Encoding.UTF8.GetString(pluginStructByte);

#if DEBUG
            Console.WriteLine($"Loader Struct: {loaderStruct}");
            Console.WriteLine($"Plugin Struct: {pluginStruct}");
#endif

            MD5 md5 = MD5.Create();

            byte[] localHash = md5.ComputeHash(src, 32, src.Length - 32);

            md5.Dispose();

#if DEBUG
            Console.WriteLine($"Local Hash Code: {Encoding.UTF8.GetString(localHash)}");
#endif

            for (int i = 0; i < 16; i++)
            {
                if (!hash[i].Equals(localHash[i]))
                    throw new Exception("MD5 Hash Error.");
            }

            return Tuple.Create(loaderStruct, pluginStruct);
        }
    }
}
