<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.dll</Reference>
  <NuGetReference>InstagramApiSharp</NuGetReference>
  <Namespace>InstagramApiSharp</Namespace>
  <Namespace>InstagramApiSharp.API</Namespace>
  <Namespace>InstagramApiSharp.API.Builder</Namespace>
  <Namespace>InstagramApiSharp.API.Processors</Namespace>
  <Namespace>InstagramApiSharp.API.Versions</Namespace>
  <Namespace>InstagramApiSharp.Classes</Namespace>
  <Namespace>InstagramApiSharp.Classes.Android.DeviceInfo</Namespace>
  <Namespace>InstagramApiSharp.Classes.Models</Namespace>
  <Namespace>InstagramApiSharp.Classes.Models.Business</Namespace>
  <Namespace>InstagramApiSharp.Classes.Models.Hashtags</Namespace>
  <Namespace>InstagramApiSharp.Classes.ResponseWrappers</Namespace>
  <Namespace>InstagramApiSharp.Classes.ResponseWrappers.BaseResponse</Namespace>
  <Namespace>InstagramApiSharp.Classes.ResponseWrappers.Business</Namespace>
  <Namespace>InstagramApiSharp.Classes.ResponseWrappers.Web</Namespace>
  <Namespace>InstagramApiSharp.Classes.SessionHandlers</Namespace>
  <Namespace>InstagramApiSharp.Enums</Namespace>
  <Namespace>InstagramApiSharp.Helpers</Namespace>
  <Namespace>InstagramApiSharp.Logger</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Web</Namespace>
  <Namespace>System.Web.Caching</Namespace>
  <Namespace>System.Web.Compilation</Namespace>
  <Namespace>System.Web.Configuration</Namespace>
  <Namespace>System.Web.Configuration.Internal</Namespace>
  <Namespace>System.Web.Globalization</Namespace>
  <Namespace>System.Web.Handlers</Namespace>
  <Namespace>System.Web.Hosting</Namespace>
  <Namespace>System.Web.Instrumentation</Namespace>
  <Namespace>System.Web.Mail</Namespace>
  <Namespace>System.Web.Management</Namespace>
  <Namespace>System.Web.ModelBinding</Namespace>
  <Namespace>System.Web.Profile</Namespace>
  <Namespace>System.Web.Routing</Namespace>
  <Namespace>System.Web.Security</Namespace>
  <Namespace>System.Web.Security.AntiXss</Namespace>
  <Namespace>System.Web.SessionState</Namespace>
  <Namespace>System.Web.UI</Namespace>
  <Namespace>System.Web.UI.Adapters</Namespace>
  <Namespace>System.Web.UI.HtmlControls</Namespace>
  <Namespace>System.Web.UI.WebControls</Namespace>
  <Namespace>System.Web.UI.WebControls.Adapters</Namespace>
  <Namespace>System.Web.UI.WebControls.WebParts</Namespace>
  <Namespace>System.Web.Util</Namespace>
  <Namespace>System.Web.WebSockets</Namespace>
  <Namespace>System.Net</Namespace>
</Query>

string basePath = @"C:\Users\yoann\Source\Repos\naia\backup_insta";
public void Main()
{
	//RipPosts();
	RipPictures();
}

public void RipPictures()
{
	var qry =
		from x in Directory.GetFiles(basePath, "*_post.txt")
		select x.Replace("_post.txt", string.Empty);

	foreach (var path in qry)
	{		
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		var medias = 
			File.ReadAllLines($"{path}_medias.txt")
			.Concat(File.ReadAllLines($"{path}_photos.txt"))
			.Dump();
		
		var i = 0;
		foreach (var media in medias)
		{
			i++;
			new WebClient().DownloadFile(media, Path.Combine(path, $"{i}.jpg"));
		}
	}

	//qry.Dump();
}


public void RipPosts()
{
	var api = InstaApiBuilder.CreateBuilder().SetUser(new UserSessionData() { UserName = "strokannn", Password = "CassiuS6034" }).Build();
	var xx = api.LoginAsync().Result;
	//api.DiscoverProcessor.SearchPeopleAsync("naia_nastasia").Result.Dump();
	//api.GetCurrentUserAsync().Result.Dump();

	var set = api.UserProcessor.GetUserMediaAsync("naia_nastasia", PaginationParameters.MaxPagesToLoad(1664)).Result.Value;
	var qry =
		from x in set
		select new
		{
			x.Caption.CreatedAtUtc,
			path = Path.Combine(basePath, x.Caption.CreatedAtUtc.ToString("yyyyMMdd_HHmmss")),
			x.Caption.Text,
			x,
			medias = x.Carousel?.Select(a => a.Images.FirstOrDefault().Uri),
			images = x.Images?.Select(a => a.Uri)
		};

	foreach (var element in qry)
	{
		try
		{
			File.WriteAllText($"{element.path}_post.txt", element.Text);
		}
		catch (Exception ex)
		{
			new { type = "CONTENT", ex, element }.Dump();
		}

		try
		{
			File.WriteAllText($"{element.path}_medias.txt", string.Empty);
			File.WriteAllText($"{element.path}_photos.txt", string.Empty);

			if (element.medias != null)
				File.AppendAllText($"{element.path}_medias.txt", string.Join(Environment.NewLine, element.medias));
			if (element.images != null)
				File.AppendAllText($"{element.path}_photos.txt", string.Join(Environment.NewLine, element.images));
		}
		catch (Exception ex)
		{
			new { type = "MEDIA", ex, element }.Dump();
		}
	}
}