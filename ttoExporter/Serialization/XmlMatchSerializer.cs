﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttoExporter.Serialization
{
    using System.IO;
    using System.Xml.Serialization;
    using System.Linq;

    /// <summary>
    /// Serializes matches to XML>
    /// </summary>
    public class XmlMatchSerializer : IMatchSerializer
    {
        /// <summary>
        /// The serializer to use.
        /// </summary>
        private XmlSerializer serializer = new XmlSerializer(typeof(Match));

        /// <summary>
        /// Serializes a match.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="match">The match to serialize</param>
        public void Serialize(Stream stream, Match match)
        {
            this.serializer.Serialize(stream, match);
        }

        /// <summary>
        /// Deserializes a match.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The deserialized match.</returns>
        public Match Deserialize(Stream stream)
        {
            Match temp = (Match)this.serializer.Deserialize(stream);
            foreach (Playlist p in temp.Playlists)
                p.Match = temp;

            return temp;
        }
    }
}
