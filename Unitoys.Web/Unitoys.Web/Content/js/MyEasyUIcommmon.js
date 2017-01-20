function SetDate(oid) {
    var now = new Date();
    var year = now.getFullYear(); //getFullYear getYear
    var month = now.getMonth();
    var date = now.getDate();
    var day = now.getDay();
    var hour = now.getHours();
    var minu = now.getMinutes();
    var sec = now.getSeconds();

    month = month + 1;
    if (month < 10) month = "0" + month;
    if (date < 10) date = "0" + date;
    if (hour < 10) hour = "0" + hour;
    if (minu < 10) minu = "0" + minu;
    if (sec < 10) sec = "0" + sec;

    var time = "";
    time = year + "-" + month + "-" + date + "-" + " " + hour + ":" + minu + ":" + sec;
    $("#" + oid).datetimebox("setValue", time)

}
function strDateTime(str) {
    var reg = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/;
    var r = str.match(reg);
    if (r == null) return false;
    var d = new Date(r[1], r[3] - 1, r[4], r[5], r[6], r[7]);
    return (d.getFullYear() == r[1] && (d.getMonth() + 1) == r[3] && d.getDate() == r[4] && d.getHours() == r[5] && d.getMinutes() == r[6] && d.getSeconds() == r[7]);
}
function Msgshow(msg) {
    $.messager.show({
        title: '提示',
        msg: msg,
        showType: 'show'
    });
}
function Msgslide(msg) {
    $.messager.show({
        title: '提示',
        msg: msg,
        timeout: 3000,
        showType: 'slide'
    });
}
function Msgfade(msg) {
    $.messager.show({
        title: '提示',
        msg: msg,
        timeout: 3000,
        showType: 'fade'
    });
}
function getSelectedArr() {
    var ids = [];
    var rows = grid.datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].ID);
    }
    return ids;
}
function getSelectedID() {
    var ids = getSelectedArr();
    return ids.join(',');
}
//弹出信息窗口 title:标题 msgString:提示信息 msgType:信息类型 [error,info,question,warning]
function Msgalert(title, msgString, msgType) {
    $.messager.alert(title, msgString, msgType);
}
//当提交信息的触发
var successCallback = function (result) {
    //result为请求处理后的返回值    
    var result = eval('(' + result + ')');
    if (result.Success) {
        $.messager.show({
            title: '成功',
            msg: result.Msg,
            timeout: 2000,
            showType: 'fade'
        });
        dlg_Edit.dialog('close'); //dlg_Edit与其他文件中的一致  才能公共调用 
        grid.datagrid('reload'); //grid变量要与其他文件中的一致   
    } else {
        $.messager.show({
            title: '错误提示',
            msg: result.Msg
        });
    }
}
JSON.find = function (obj, key, val) {
    var objects = [];
    for (var i in obj) {
        if (!obj.hasOwnProperty(i)) continue;
        if (typeof obj[i] == 'object') {
            objects = objects.concat(JSON.find(obj[i], key, val));
        } else if (i == key && obj[key] == val) {
            objects.push(obj);
        }
    }
    return objects;
}
Array.del = function (ary, obj) {   
    var i = ary.indexOf(obj);       
    if (i != -1) {
        ary.splice(i, 1);
    }
}

function formatDateTimeInt(value, rec, index) {
    if (!value) {
        return "";
    }
    var unixTimestamp = new Date(value * 1000);
    return unixTimestamp.getFullYear() + '/' + (unixTimestamp.getMonth() + 1) + '/' + unixTimestamp.getDate() + ' ' + unixTimestamp.getHours() + ':' + unixTimestamp.getMinutes() + ':' + unixTimestamp.getSeconds();
}