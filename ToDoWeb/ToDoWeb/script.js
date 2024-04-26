function deleteQuest(){
        $.get("", function (data) {
            $("p").html(data);
        });
}