VAR girl = "Sprites/Perhta3"
VAR perchta = ""

=== Perchta ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<wave>oho-Ho<b><size=120%>-HO</size=120%><size=140%>-HO</size=140%><size=160%>-HO</size=160%><size=180%>-HO!</size=180%></b></wave>
# speaker:<b><size=120%>Perchta</size=120%></b>
What do we have <i>hereee~</i> what a <i>child</i>, <b>what a lovely girl!</b>
# speaker:<b><size=120%>Perchta</size=120%></b>
You've cleaned the room well, <b><size=140%>oh, the whole room!</size=140%></b> See no dust, <b>no dust!</b>
# speaker:<b><size=120%>Perchta</size=120%></b>
… now, your <b>Mother</b> has been a naughty girl. All these spells and sigils kept us away for a long time.
# speaker:<b><size=120%>Perchta</size=120%></b>
All too hide all these filth and schnapps from us, you see? <size=160%>Oh</size=160%><size=140%>-HO</size=140%><size=120%>-Ho</size=120%>, but you, you're a good girl, ain't you?
# speaker:<b><size=120%>Perchta</size=120%></b>
You <b><size=140%>will</size=140%></b> clean the <shiver>filth</shiver> and spare the <shiver>drink</shiver>.
# speaker:<b>Girl</b>
<color="black">.</color="black">
    + [<i>So that's why you've never visited us before…</i>]
        -> visited
    + [<i>I will.</i>]
        -> will

=== visited ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=180%>Hush</size=180%></b> now, child!
# speaker:<b><size=120%>Perchta</size=120%></b>
You have a room to clean, so you'd <b>better</b> start doing so.
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><shiver>Older sister</shiver></b> will see you soon.

-> END

=== will ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<size=160%>Oh</size=160%><size=140%>-HO</size=140%><size=120%>-Ho</size=120%>, such a <i>sweet</i> child you are!
# speaker:<b><size=120%>Perchta</size=120%></b>
 Now go, prove that the apple fell <i>further</i> from this <i>filthy</i> tree.

-> END