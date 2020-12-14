$.fn.extend({
    bindData: function (data, id, name)
    {
        var htmls = '<option value="0">全部</option>';
        for (var i = 0; i < data.length; i++) {
            htmls += ' <option value="' + eval('data[i].' + id) + '">' + eval('data[i].' + name) + '</option>';
        }
        $(this).html(htmls);
    }
});


function LoadCombobox(selector,data, valueField, textField) {
    var oldvalue = $(selector).combobox('getValue');

    $(selector).combobox({ valueField: valueField, textField: textField });

    data.unshift(JSON.parse('{"' + valueField + '":0,"' + textField + '":"全部"}'));

    $(selector).combobox('loadData', data);
    $(selector).combobox("enable");

    var ishave = false;
    $.each($(selector).combobox('getData'), function (i, n) {
        if (eval('n.' + valueField) == oldvalue) {
            ishave = true;
            return false;
        }
    });

    if (!ishave) {
        $(selector).combobox('setValue', 0);
    } else {
        $(selector).combobox('setValue', oldvalue);
    }
}

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
}