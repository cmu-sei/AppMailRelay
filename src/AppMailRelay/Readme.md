# AppMailRelay API

1. Get an `x-api-key` for your app from the relay server admins.

2. To send mail, `POST` a message to `##URL##` with an `X-API-KEY` header and the following payload:
```json
{
    messageId: "optional for client-side tracking",
    from: "optional sender address",
    to: "address [; address]+",
    cc: "address [; address]+",
    subject: "message subject",
    text: "body text",
    html: "body html",
    bccSender: false
}
```

* Address format can be `sender@some.ws` or `Sender Name <sender@some.ws>`
* If a `from` address is provided, ensure it is valid at the mail server.
(Only send `from` when wanting to overriding the default relay sender.)

3. Expect a MailMessageStatus response as follows:
```json
{
    messageId: "",
    referenceId: "",
    status: "unknown | pending | success | failure",
    timestamp: "status moment in utc"
}
```

4. If tracking message completion, `GET` from `##URL##/<referenceId>` or `##URL##?id=<referenceId>`.
Expect another MailMessageStatus object.
Status is only maintained for 10 minutes, so you'll get status.unknown if checking too late.
