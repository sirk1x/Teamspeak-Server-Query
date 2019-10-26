using System;
using System.Collections.Generic;
using System.Text;

namespace MyLittleTeamspeakServerQuery.Dataset
{
    public class ServerInfo
    {
        public string virtualserver_unique_identifier { get; set; }
        public string virtualserver_name { get; set; }
        public string virtualserver_welcomemessage { get; set; }
        public string virtualserver_platform { get; set; }
        public string virtualserver_version { get; set; }
        public int virtualserver_machine_id { get; set; }
        public uint virtualserver_maxclients { get; set; }
        public string virtualserver_password { get; set; }
        public uint virtualserver_clientsonline { get; set; }
        public uint virtualserver_channelsonline { get; set; }
        public uint virtualserver_created { get; set; }
        public uint virtualserver_uptime { get; set; }
        public uint virtualserver_codec_encryption_mode { get; set; }
        public string virtualserver_hostmessage { get; set; }
        public uint virtualserver_hostmessage_mode { get; set; }
        public string virtualserver_filebase { get; set; }
        public uint virtualserver_default_server_group { get; set; }
        public uint virtualserver_default_channel_group { get; set; }
        public uint virtualserver_flag_password { get; set; }
        public uint virtualserver_default_channel_admin_group { get; set; }
        public ulong virtualserver_max_download_total_bandwidth { get; set; }
        public ulong virtualserver_max_upload_total_bandwidth { get; set; }
        public string virtualserver_hostbanner_url { get; set; }
        public string virtualserver_hostbanner_gfx_url { get; set; }
        public uint virtualserver_hostbanner_gfx_interval { get; set; }
        public uint virtualserver_complain_autoban_count { get; set; }
        public int virtualserver_complain_autoban_time { get; set; }
        public uint virtualserver_complain_remove_time { get; set; }
        public uint virtualserver_min_clients_in_channel_before_forced_silence { get; set; }
        public double virtualserver_priority_speaker_dimm_modificator { get; set; }
        public uint virtualserver_id { get; set; }
        public uint virtualserver_antiflood_points_tick_reduce { get; set; }
        public uint virtualserver_antiflood_points_needed_command_block { get; set; }
        public uint virtualserver_antiflood_points_needed_ip_block { get; set; }
        public uint virtualserver_client_connections { get; set; }
        public uint virtualserver_query_client_connections { get; set; }
        public string virtualserver_hostbutton_tooltip { get; set; }
        public string virtualserver_hostbutton_url { get; set; }
        public string virtualserver_hostbutton_gfx_url { get; set; }
        public uint virtualserver_queryclientsonline { get; set; }
        public ulong virtualserver_download_quota { get; set; }
        public ulong virtualserver_upload_quota { get; set; }
        public ulong virtualserver_month_bytes_downloaded { get; set; }
        public ulong virtualserver_month_bytes_uploaded { get; set; }
        public ulong virtualserver_total_bytes_downloaded { get; set; }
        public ulong virtualserver_total_bytes_uploaded { get; set; }
        public ushort virtualserver_port { get; set; }
        public uint virtualserver_autostart { get; set; }
        public uint virtualserver_needed_identity_security_level { get; set; }
        public uint virtualserver_log_client { get; set; }
        public uint virtualserver_log_query { get; set; }
        public uint virtualserver_log_channel { get; set; }
        public uint virtualserver_log_permissions { get; set; }
        public uint virtualserver_log_server { get; set; }
        public uint virtualserver_log_filetransfer { get; set; }
        public uint virtualserver_min_client_version { get; set; }
        public uint virtualserver_icon_id { get; set; }
        public uint virtualserver_reserved_slots { get; set; }
        public double virtualserver_total_packetloss_speech { get; set; }
        public double virtualserver_total_packetloss_keepalive { get; set; }
        public double virtualserver_total_packetloss_control { get; set; }
        public double virtualserver_total_packetloss_total { get; set; }
        public double virtualserver_total_ping { get; set; }
        public string virtualserver_ip { get; set; }
        public uint virtualserver_weblist_enabled { get; set; }
        public uint virtualserver_ask_for_privilegekey { get; set; }
        public uint virtualserver_hostbanner_mode { get; set; }
        public uint virtualserver_channel_temp_delete_delay_default { get; set; }
        public uint virtualserver_min_android_version { get; set; }
        public uint virtualserver_min_ios_version { get; set; }
        public uint virtualserver_antiflood_points_needed_plugin_block { get; set; }
        public string virtualserver_status { get; set; }
        public ulong connection_filetransfer_bandwidth_sent { get; set; }
        public ulong connection_filetransfer_bandwidth_received { get; set; }
        public ulong connection_filetransfer_bytes_sent_total { get; set; }
        public ulong connection_filetransfer_bytes_received_total { get; set; }
        public ulong connection_packets_sent_speech { get; set; }
        public ulong connection_bytes_sent_speech { get; set; }
        public ulong connection_packets_received_speech { get; set; }
        public ulong connection_bytes_received_speech { get; set; }
        public ulong connection_packets_sent_keepalive { get; set; }
        public ulong connection_bytes_sent_keepalive { get; set; }
        public ulong connection_packets_received_keepalive { get; set; }
        public ulong connection_bytes_received_keepalive { get; set; }
        public ulong connection_packets_sent_control { get; set; }
        public ulong connection_bytes_sent_control { get; set; }
        public ulong connection_packets_received_control { get; set; }
        public ulong connection_bytes_received_control { get; set; }
        public ulong connection_packets_sent_total { get; set; }
        public ulong connection_bytes_sent_total { get; set; }
        public ulong connection_packets_received_total { get; set; }
        public ulong connection_bytes_received_total { get; set; }
        public ulong connection_bandwidth_sent_last_second_total { get; set; }
        public ulong connection_bandwidth_sent_last_minute_total { get; set; }
        public ulong connection_bandwidth_received_last_second_total { get; set; }
        public ulong connection_bandwidth_received_last_minute_total { get; set; }

        public Client[] clients { get; set; }

        public Channel[] channels { get; set; }

        public ServerGroup[] groups { get; set; }


        public List<Tuple<uint, string>> icons { get; set; }
    }
}
public enum VirtualServerStatus { Online, Virtual, OtherInstance, None }
public enum HostBannerMode
{
    No_Adjust = 0,
    Adjust_Ignore_Aspect = 1,
    Adjust_Keep_Aspect = 2,
}
public enum HostMessageMode
{
    /// <summary>
    /// dont display anything
    /// </summary>
    HostMessageModeNone = 0,
    /// <summary>
    /// display message in chatlog
    /// </summary>
    HostMessageModeLog = 1,
    /// <summary>
    /// display message in modal dialog
    /// </summary>
    HostMessageModeModal = 2,
    /// <summary>
    /// display message in modal dialog and close connection
    /// </summary>
    HostMessageModeModalQuit = 3
}
