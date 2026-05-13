VAR girl = "Sprites/Perhta1"
VAR perchta = ""

=== Perchta ===
# speaker:<b><size=120%>Perchta</size=120%></b>
Oh, <i>little child</i>. Pardon my visit.
# speaker:<b><size=120%>Perchta</size=120%></b>
I believe you shouldn't have seen all this.
# speaker:<b><size=120%>Perchta</size=120%></b>
As you see, I came for your <i>mommy…</i>
# speaker:<b>Girl</b>
<color="black">.</color="black">
    + [<i><shiver>W-w-what are you…?</shiver></i>]
        -> what
    + [<i><shiver>W-w-why…?</shiver></i>]
        -> why
        
=== what ===
# speaker:<b><size=120%>Perchta</size=120%></b>
We are <b>Schnabelperchten</b>, dear.
# speaker:<b><size=120%>Perchta</size=120%></b>
We seek out the folk that live in filth…
# speaker:<b><size=120%>Perchta</size=120%></b>
Like <b>you and your Mommy</b> are.
-> kill_me

=== why ===
# speaker:<b><size=120%>Perchta</size=120%></b>
Because your <i>mommy</i> is rotten, of course!
# speaker:<b><size=120%>Perchta</size=120%></b>
She dared to turn <i>this little</i> house into a <b><size=120%>BUGHOLE!</size=120%><b>
-> kill_me

=== kill_me ===
# speaker:<b>Girl</b>
<color="black">.</color="black">
    + [<i><shiver>W-w-will you kill me now?</shiver></i>]
        -> frightened
    + [<i><shiver>It's… It's not her fault!</shiver></i>]
        -> fault

=== frightened ===
# speaker:<b><size=120%>Perchta</size=120%></b>
Kill you? I should, really.
# speaker:<b><size=120%>Perchta</size=120%></b>
Yet it is not your fault that this house is a mess, is it?
# speaker:<b><size=120%>Perchta</size=120%></b>
<size=75%><wave>And there is so much schnapps left here…</wave></size=75%>
-> end_dialogue

=== fault
# speaker:<b><size=120%>Perchta</size=120%></b>
Is it now?! Maybe it is your fault then?
-> end_dialogue

=== end_dialogue === 
# speaker:<b><size=120%>Perchta</size=120%></b>
I have an <shiver>idea</shiver>, <i>little one</i>.
# speaker:<b><size=120%>Perchta</size=120%></b>
Clean all the mess here, and then I, with my sisters, will let you live. 
# speaker:<b><size=120%>Perchta</size=120%></b>
<b>They</b> will decide <b>if you are worthy of life</b> later tonight. Be a good <i>little child.</i>
# speaker:<b><size=120%>Perchta</size=120%></b>
Oh, <i>and be a sweetie</i>, bring us leftovers of that <shiver><b>delicious schnapps</b></shiver> that your mother had.
# speaker:<b><size=120%>Perchta</size=120%></b>
Don't be scared now, <i>little one</i>, all these shadows are nothing but rubbish.
# speaker:<b><size=120%>Perchta</size=120%></b>
Yet I believe your imagination will be quite <b><i>humorous</i></b> after <b><shiver>what</shiver></b> you've seen.
# speaker:<b><size=120%>Perchta</size=120%></b>
<i><wave>Ehe-he-he-he-he</wave></i>

-> END













