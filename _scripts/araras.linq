<Query Kind="Program">
  <NuGetReference>HtmlAgilityPack</NuGetReference>
  <Namespace>HtmlAgilityPack</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

string fromPath = $"C:/Users/yoann/Source/Repos/www.unefilleetlemonde.com/";
void Main()
{
	Directory.GetFiles(fromPath, "*.html", SearchOption.AllDirectories)
	.Select(a => Path.GetFileName(a))
	.Select(a => $"<p><a href='{a}'>{a}</a></p>")
	.Dump();
	//	var set = Directory.GetFiles(fromPath, "*.html", SearchOption.AllDirectories)
	//	.Select(Parse)
	//	.Where(a => a != null)
	//	.ToList()	
	//	.Dump()
	//	;
}

public object Parse(string uri)
{
	try
	{
		var rawdata = File.ReadAllText(uri);
		var html = new HtmlDocument();
		html.LoadHtml(rawdata);
		var title = html.DocumentNode.Descendants("h1")
		.SingleOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("headline img-headline"))
		.InnerHtml
		.Trim();
		var content = html.DocumentNode.Descendants("div")
		.Single(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("content overlap"))
		.Descendants("div").First()
		.Descendants("div").First()
		.Descendants("div").First()
		.Descendants("div").First()
		.InnerHtml
		.Trim();
		var created =
		html.DocumentNode.Descendants("meta")
		.Single(d => d.Attributes.Contains("property") && d.Attributes["property"].Value.Contains("article:published_time"))
		.Attributes["content"].Value.Trim()
		;
		var obj = new { x = Convert.ToDateTime(created).ToString("yyyyMMdd") + "-" + uri.Replace(fromPath, "").Replace("\\index.html", "") + ".html", uri, title, content, created, };
		File.WriteAllText(Path.Combine(fromPath, "_araras", obj.x), obj.content);
		return obj;
	}
	catch (Exception ex)
	{
		//ex.Dump();
		return null;
	}
}