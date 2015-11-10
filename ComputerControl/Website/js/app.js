$(document).on("ready", function()
{
	Communication.Init();
});

var Communication = {
	
	Socket: null,
	
	Init: function () {

	    Communication.Socket = new WebSocket("ws://" + window.location.hostname + ":5050");
	    Communication.Socket.onopen = Communication.OnOpen;
	    Communication.Socket.onmessage = Communication.OnMessage;
	},

	OnOpen: function ()
	{
	    Communication.Send({ Type: "Init" });
	},
	
	OnMessage: function(event)
	{
		var data = JSON.parse(event.data);
		window["HandleData"][data.Type](data.Data);		
	},

	Send: function(data)
	{
		Communication.Socket.send(JSON.stringify(data));
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

	    $("#Ram-Total").text(parseFloat(data.Total / 1024).toFixed(2) + " GB");
	    $("#Ram-Used").text(parseFloat(used / 1024).toFixed(2) + " GB");
	    $("#Ram-Free").text(parseFloat(data.Avaliable / 1024).toFixed(2) + " GB");
	},
	
	DiskInfo: function(data)
	{
		$(data).each(function(i, element) {
		    $("#DiskActivity-" + element.Id).css("width", element.Usage + "%");
			$("#DiskActivityText-" + element.Id).text(element.Letter + ": " + element.Usage + "%");
		});
	},

	SystemInfo: function(data) {

	    var content;

	    for (var i = 0; i < data.Cpu; i++) {
	        content = $("#CpuProgressTemplate").html();

	        $("#CpuLoadContainer").append(content.replace(new RegExp("ID", "g"), i).replace(new RegExp("ID-INC", "g"), i + 1));
	    }

        for (var j = 0; j < data.Disk; j++) {
            content = $("#DiskActivityTemplate").html();

            if (data.Disk === 1) {
                content = content.replace("6", "12");
            }

            $("#DiskActivityContainer").append(content.replace(new RegExp("ID", "g"), j));
        }
	}
}