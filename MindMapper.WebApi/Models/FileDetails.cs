﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MindMapper.WebApi.Models
{

    public class FileDetails
    {

        public int ID { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public FileType FileType { get; set; }
    }
}
