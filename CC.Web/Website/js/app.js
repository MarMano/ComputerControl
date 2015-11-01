$(document).on("ready", function()
{
	Communication.Init();
});

var Communication = {
	
	Socket: null,
	
	Init: function()
	{
		Communication.Socket = new WebSocket("ws://" + window.location.hostname + ":5050");
		Communication.Socket.onmessage = Communication.OnMessage;
	},
	
	OnMessage: function(event)
	{
		var data = JSON.parse(event.data);
		window["HandleData"][data.Type](data.Data);		
	},
	
	Send: function(data)
	{
		
	}	
};

var HandleData = {
	CpuInfo: function(data)
	{
		$(data.SingleCpu).each(function(i, element)
		{
			$("#CpuLoadText-" + element.Id).text("Cpu #" + (element.Id + 1) + ": " + element.Load + "%");
			$("#CpuLoad-" + element.Id).css("width", element.Load + "%");
		});
		
		$("#CpuLoadText-Total").text("Total Load: " + data.Total.Load + "%");
		$("#CpuLoad-Total").css("width", data.Total.Load + "%");
	},
	
	RamInfo: function(data)
	{
		var used = data.Total - data.Avaliable;
		var percentage = Math.round(used / data.Total * 100);
		
		$("#Ram-PercentText").text("Ram Usage: " + percentage + "%");
		$("#Ram-Percent").css("width", percentage + "%");
		
		$("#Ram-Total").text(parseFloat(data.Total / 1024).toFixed(2) + " GB")
		$("#Ram-Used").text(parseFloat(used / 1024).toFixed(2) + " GB")
		$("#Ram-Free").text(parseFloat(data.Avaliable / 1024).toFixed(2) + " GB")
	},
	
	DiskInfo: function(data)
	{
		$(data).each(function(i, element)
		{
			$("#DiskActivity-" + element.Id).css("width", element.Usage + "%")
			$("#DiskActivityText-" + element.Id).text(element.Letter + ": " + element.Usage + "%");
		});
	}
}