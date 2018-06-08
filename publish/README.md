# 太阳湾软件微信域名检测新系统
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/gemgin/WeChatTools/pulls)
[![GitHub stars](https://img.shields.io/github/stars/gemgin/WeChatTools.svg?style=social&label=Stars)](https://github.com/gemgin/WeChatTools)
[![GitHub forks](https://img.shields.io/github/forks/gemgin/WeChatTools.svg?style=social&label=Fork)](https://github.com/gemgin/WeChatTools)

交流QQ群：41977413

## 微信域名检测接口升级内容--20180608

- 在微信浏览网站,出现`如需浏览,请长按网址复制后使用浏览器访问`，检测结果`屏蔽`

- 在微信浏览网站,出现`将要访问www.dddds.xyz 非微信官方网页,继续访问将转换成手机预览模式`，检测结果`屏蔽`

- 在微信浏览站点部分链接,链接屏蔽,而域名没屏蔽,针对这个链接检测结果`屏蔽`


## 详细情况

> 如需浏览,请长按网址复制后使用浏览器访问,`http://www.pianyi9.cn`

- 原因:一般是淘宝客站点

```
Connection: keep-alive
Location: https://szsupport.weixin.qq.com/cgi-bin/mmsupport-bin/readtemplate?t=w_redirect_taobao&url=http%3A%2F%2Fwww.pianyi9.cn&pass_ticket=yJhJV1kLUCmaQj2AhjEzZWYbmqfeM9YifFTtD5zplYQh9lPqtTPRet6PZyw0ZJ92&wechat_real_lang=zh_CN
Cache-Control: no-cache, must-revalidate
Strict-Transport-Security: max-age=31536000 ; includeSubDomains
Content-Length: 298

{
"BaseResponse": {
"Ret": 0,
"ErrMsg": ""
}
,
"FullURL": "https://szsupport.weixin.qq.com/cgi-bin/mmsupport-bin/readtemplate?t=w_redirect_taobao&url=http%3A%2F%2Fwww.pianyi9.cn&pass_ticket=yJhJV1kLUCmaQj2AhjEzZWYbmqfeM9YifFTtD5zplYQh9lPqtTPRet6PZyw0ZJ92&wechat_real_lang=zh_CN",
"ActionCode": 7
}

```
![请长按网址复制后使用浏览器访问](http://images.cnblogs.com/cnblogs_com/Gemgin/12212241/o_ruxuliulan.jpg)

> 将要访问`www.abc1234567.xyz`非微信官方网页,继续访问将转换成手机预览模式

- 原因:域名没有备案

```
Connection: keep-alive
Location: https://weixin110.qq.com/cgi-bin/mmspamsupport-bin/newredirectconfirmcgi?main_type=1&evil_type=100&source=2&url=http%3A%2F%2Fabc1234567.xyz%3Fnsukey%3D9igfmuL08xZx3lW57Jp6R9isMJSlSUgikxbBT8QP82TeBrLN2KaMLsi4vaDtt%252F9cZ4tRztUqewitLqH%252BteNm8D2CjXECvPsiYLDAhfD1T%252B1QEGUuIkNRKOUKPfdL%252F1pyL5n07rbRBKn3RYT1DyQyacjPEYoj8C06KABuKwDkZucO2EjzCKcZZoCJY6vmQaX1lZ0hJ4y38k0BRaI5O%252FdTJw%253D%253D&scene=1&devicetype=webwx&exportkey=A023DFB4cSqkE5yVJ0Q932c%3D&pass_ticket=yJhJV1kLUCmaQj2AhjEzZWYbmqfeM9YifFTtD5zplYQh9lPqtTPRet6PZyw0ZJ92&wechat_real_lang=zh_CN
Cache-Control: no-cache, must-revalidate
Strict-Transport-Security: max-age=31536000 ; includeSubDomains
Content-Length: 636

{
"BaseResponse": {
"Ret": 0,
"ErrMsg": ""
}
,
"FullURL": "https://weixin110.qq.com/cgi-bin/mmspamsupport-bin/newredirectconfirmcgi?main_type=1&evil_type=100&source=2&url=http%3A%2F%2Fabc1234567.xyz%3Fnsukey%3D9igfmuL08xZx3lW57Jp6R9isMJSlSUgikxbBT8QP82TeBrLN2KaMLsi4vaDtt%252F9cZ4tRztUqewitLqH%252BteNm8D2CjXECvPsiYLDAhfD1T%252B1QEGUuIkNRKOUKPfdL%252F1pyL5n07rbRBKn3RYT1DyQyacjPEYoj8C06KABuKwDkZucO2EjzCKcZZoCJY6vmQaX1lZ0hJ4y38k0BRaI5O%252FdTJw%253D%253D&scene=1&devicetype=webwx&exportkey=A023DFB4cSqkE5yVJ0Q932c%3D&pass_ticket=yJhJV1kLUCmaQj2AhjEzZWYbmqfeM9YifFTtD5zplYQh9lPqtTPRet6PZyw0ZJ92&wechat_real_lang=zh_CN",
"ActionCode": 7
}

``` 
![非微信官方网页](http://images.cnblogs.com/cnblogs_com/Gemgin/12212241/o_jiangfangwen.jpg)

> 在微信浏览站点部分链接,链接屏蔽,而域名没屏蔽

- 原因:大量群发此链接(常见于跳转链接)

```
Connection: keep-alive
Location: https://weixin110.qq.com/cgi-bin/mmspamsupport-bin/newredirectconfirmcgi?main_type=2&evil_type=20&source=2&url=http%3A%2F%2Fopenid.auth.te1111.com%2Fopen.php%3Fid%3D100%26sto%3DYUhSMGNEb3ZMMlJ2Ym1kbmRXRnVMbU5wZEhrdWVHbGhibWgxYjNSMVlXNHVZMjl0THpNd01TNXdhSEEvY0dsa1BUVW1kSGx3WlQwd0ptWnliMjA5ZDNnbWRtbGtQVEV6&exportkey=AzNjKsfz%2FFneKW%2BVmUk%2FkLU%3D&pass_ticket=yJhJV1kLUCmaQj2AhjEzZWYbmqfeM9YifFTtD5zplYQh9lPqtTPRet6PZyw0ZJ92&wechat_real_lang=zh_CN
Cache-Control: no-cache, must-revalidate
Strict-Transport-Security: max-age=31536000 ; includeSubDomains
Content-Length: 528

{
"BaseResponse": {
"Ret": 0,
"ErrMsg": ""
}
,
"FullURL": "https://weixin110.qq.com/cgi-bin/mmspamsupport-bin/newredirectconfirmcgi?main_type=2&evil_type=20&source=2&url=http%3A%2F%2Fopenid.auth.te1111.com%2Fopen.php%3Fid%3D100%26sto%3DYUhSMGNEb3ZMMlJ2Ym1kbmRXRnVMbU5wZEhrdWVHbGhibWgxYjNSMVlXNHVZMjl0THpNd01TNXdhSEEvY0dsa1BUVW1kSGx3WlQwd0ptWnliMjA5ZDNnbWRtbGtQVEV6&exportkey=AzNjKsfz%2FFneKW%2BVmUk%2FkLU%3D&pass_ticket=yJhJV1kLUCmaQj2AhjEzZWYbmqfeM9YifFTtD5zplYQh9lPqtTPRet6PZyw0ZJ92&wechat_real_lang=zh_CN",
"ActionCode": 7
}

```

## 注意事项
- 域名检测试用接口 [http://wx.rrbay.com/pro/wxUrlCheck.ashx?url=http://www.teu7.cn](http://wx.rrbay.com/pro/wxUrlCheck.ashx?url=http://www.teu7.cn "域名检测试用接口")
```
 {"State":true, "Data":"www.teu7.cn","Msg":"屏蔽"}
 {"State":true, "Data":"jingdong.com","Msg":"正常"}
 {"State":false,"Data":"jingdong.com","Msg":"非法访问，访问被拒绝,进qq群交流:41977413"}
 {"State":false,"Data":"jingdong.com","Msg":"歇一歇,访问太快了,进qq群交流:41977413"}
 {"State":false,"Data":"jingdong.com","Msg":"服务暂停,请联系管理员!"}
```
- 域名检测界面：http://wx.rrbay.com/

- 微信域名检测可以检查域名也可以检测链接,api检测链接的时,`url参数一定要编码`
 
