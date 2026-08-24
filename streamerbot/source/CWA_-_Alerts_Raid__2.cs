using System;
using System.Linq;

public class CPHInline
{
    public bool Execute()
    {
        var scene = CPH.GetGlobalVar<string>("ShoutOutScene", true);
        var source = CPH.GetGlobalVar<string>("ShoutOutBrowserSource", true);
        var videoplayerfile = CPH.GetGlobalVar<string>("ShoutOutFile", true);
        string userName = args["targetUser"].ToString();
        var allClips = CPH.GetClipsForUser(userName);
        if (allClips.Count == 0)
        {
            CPH.SendMessage("This streamer doesn't have any clips! :(");
            return false;
        }

        var randomClip = allClips.OrderBy(c => Guid.NewGuid()).First();
        CPH.SetArgument("randomClipBroadcaster", randomClip.BroadcasterName);
        CPH.SetArgument("randomClipTitle", randomClip.Title);
        CPH.SetArgument("randomClipUrl", randomClip.Url);
        CPH.SetArgument("randomClipDuration", randomClip.Duration);
        CPH.SetArgument("randomClipUser", randomClip.CreatorName);
        CPH.SetArgument("randomClipViewCount", randomClip.ViewCount);
        CPH.SetArgument("randomClipThumbnailUrl", randomClip.ThumbnailUrl);
        // Create embed iframe from the URL - not currently used, but leaving in place should Twitch API's / naming system change so we can update with HTML update
        string embedURL = randomClip.Url;
        embedURL = embedURL.Replace("twitch.tv/", "twitch.tv/embed?clip=");
        CPH.SetArgument("randomClipEmbedUrl", embedURL);
        videoplayerfile += "?user=" + randomClip.BroadcasterName;
        videoplayerfile += "&image=" + args["targetUserProfileImageUrl"].ToString();
        videoplayerfile += "&video=" + embedURL;
        videoplayerfile += "&thumbnail_url=" + randomClip.ThumbnailUrl;
        int delay = 700 + (int)(randomClip.Duration * 1000);
        videoplayerfile += "&time=" + delay;
        CPH.ObsSetBrowserSource(scene, source, videoplayerfile);
        //Increase the delay passed to the source by 2 seconds for loading
        delay += 2000;
        CPH.Wait(delay);
        CPH.ObsSetBrowserSource(scene, source, "about:blank");
        return true;
    }
}