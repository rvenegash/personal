﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dtvla.newk2view.customerproducts
{
    public class RefCategory
    {
        [BsonId]
        //[BsonElement("id")]
        public string _id { get; set; }

        // public string _id { get; set; }
        [BsonElement("id")]
        public string id { get; set; }

        [BsonElement("name")]
        public string NAME { get; set; }

        [BsonElement("source_id")]
        public string SOURCE_ID { get; set; }
    }
}
