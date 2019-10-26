using System;
using System.Collections.Generic;
using System.Text;

namespace MyLittleTeamspeakServerQuery.Dataset
{
    public class Client
    {
        public uint clid { get; set; }
        public uint cid { get; set; }
        public uint client_database_id { get; set; }
        public string client_nickname { get; set; }
        public uint client_type { get; set; }

        public ClientProperties properties;
    }

    public class ClientProperties
    {
        public uint cid { get; set; }
        public uint client_idle_time { get; set; }
        public string client_unique_identifier { get; set; }
        public string client_nickname { get; set; }
        public string client_version { get; set; }
        public string client_platform { get; set; }
        public bool client_input_muted { get; set; }
        public bool client_output_muted { get; set; }
        public bool client_outputonly_muted { get; set; }
        public uint client_input_hardware { get; set; }
        public uint client_output_hardware { get; set; }
        public uint client_default_channel { get; set; }
        public string client_meta_data { get; set; }
        public bool client_is_recording { get; set; }
        public string client_version_sign { get; set; }
        public string client_security_hash { get; set; }
        public string client_login_name { get; set; }
        public uint client_database_id { get; set; }
        public uint client_channel_group_id { get; set; }
        public int[] client_servergroups { get; set; }
        public DateTime client_created { get; set; }
        public DateTime client_lastconnected { get; set; }
        public int client_totalconnections { get; set; }
        public bool client_away { get; set; }
        public string client_away_message { get; set; }
        public int client_type { get; set; }
        public string client_flag_avatar { get; set; }
        public int client_talk_power { get; set; }
        public bool client_talk_request { get; set; }
        public string client_talk_request_msg { get; set; }
        public string client_description { get; set; }
        public bool client_is_talker { get; set; }
        public ulong client_month_bytes_uploaded { get; set; }
        public ulong client_month_bytes_downloaded { get; set; }
        public ulong client_total_bytes_uploaded { get; set; }
        public ulong client_total_bytes_downloaded { get; set; }
        public bool client_is_priority_speaker { get; set; }
        public string client_nickname_phonetic { get; set; }
        public int client_needed_serverquery_view_power { get; set; }
        public string client_default_token { get; set; }
        public uint client_icon_id { get; set; }
        public bool client_is_channel_commander { get; set; }
        public string client_country { get; set; }
        public bool client_channel_group_inherited_channel_id { get; set; }
        public bool client_badges { get; set; }
        public string client_myteamspeak_id { get; set; }
        public string client_integrations { get; set; }
        public string client_myteamspeak_avatar { get; set; }
        public string client_signed_badges { get; set; }
        public string client_base64HashClientUID { get; set; }
        public ulong connection_filetransfer_bandwidth_sent { get; set; }
        public ulong connection_filetransfer_bandwidth_received { get; set; }
        public ulong connection_packets_sent_total { get; set; }
        public ulong connection_bytes_sent_total { get; set; }
        public ulong connection_packets_received_total { get; set; }
        public ulong connection_bytes_received_total { get; set; }
        public ulong connection_bandwidth_sent_last_second_total { get; set; }
        public ulong connection_bandwidth_sent_last_minute_total { get; set; }
        public ulong connection_bandwidth_received_last_second_total { get; set; }
        public ulong connection_bandwidth_received_last_minute_total { get; set; }
        public ulong connection_connected_time { get; set; }
    }
}
