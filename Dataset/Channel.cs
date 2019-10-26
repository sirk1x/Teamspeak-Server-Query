using System;
using System.Collections.Generic;
using System.Text;

namespace MyLittleTeamspeakServerQuery.Dataset
{
    public class Channel
    {
        public int cid { get; set; }
        public int pid { get; set; }
        public int channel_order { get; set; }
        public string channel_name { get; set; }
        public int total_clients { get; set; }
        public ChannelProperties channelproperties { get; set; }
    }
    public class ChannelProperties
    {
        public int pid { get; set; }
        public string channel_name { get; set; }
        public string channel_topic { get; set; }
        public string channel_description { get; set; }
        public string channel_password { get; set; }
        public int channel_codec { get; set; }
        public int channel_codec_quality { get; set; }
        public int channel_maxclients { get; set; }
        public int channel_maxfamilyclients { get; set; }
        public int channel_order { get; set; }
        public bool channel_flag_permanent { get; set; }
        public bool channel_flag_semi_permanent { get; set; }
        public bool channel_flag_default { get; set; }
        public bool channel_flag_password { get; set; }
        public int channel_codec_latency_factor { get; set; }
        public bool channel_codec_is_unencrypted { get; set; }
        public string channel_security_salt { get; set; }
        public int channel_delete_delay { get; set; }
        public bool channel_flag_maxclients_unlimited { get; set; }
        public bool channel_flag_maxfamilyclients_unlimited { get; set; }
        public bool channel_flag_maxfamilyclients_inherited { get; set; }
        public string channel_filepath { get; set; }
        public int channel_needed_talk_power { get; set; }
        public bool channel_forced_silence { get; set; }
        public string channel_name_phonetic { get; set; }
        public uint channel_icon_id { get; set; }
        public string channel_banner_gfx_url { get; set; }
        public int channel_banner_mode { get; set; }
    }
}
