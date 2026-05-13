VAR girl = "Sprites/Perhta2"
VAR perchta = ""

=== Perchta ===
# speaker:<b><size=120%>Perchta</size=120%></b>
mmm… I see, <b>little cub</b> can fight for its life. Rather violently.
# speaker:<b><size=120%>Perchta</size=120%></b>
What scared you so in that filthy pile of garbage you’ve cleaned?
# speaker:<b><size=120%>Perchta</size=120%></b>
What thy <i>wounded mind</i> sees there?
# speaker:<b><size=120%>Perchta</size=120%></b>
...But it matters not, <b>cub</b>. Despite everything, I smell no blood and see no dust.
# speaker:<b><size=120%>Perchta</size=120%></b>
Splendid work, <i><b>little one</b></i>.
# speaker:<b><size=120%>Perchta</size=120%></b>
yet I wonder…
# speaker:<b><size=120%>Perchta</size=120%></b>
<shiver><b>Where’s your mother’s body now, child?</shiver></b>
# speaker:<b>Girl</b>
<color="black">.</color="black">
    + [<i><shiver>I’ll bury her…</shiver></i>]
        -> bury
    + [<i><shiver>What do you want…?</shiver></i>]
        -> want

=== bury ===
# speaker:<b><size=120%>Perchta</size=120%></b>
Little wasteful cub.
# speaker:<b><size=120%>Perchta</size=120%></b>
There’s so much <shiver><b><size=120%>meat</size=120%></shiver></b> left… Yet it is not me who hunted her down.

-> end_dialogue

=== want ===
# speaker:<b><size=120%>Perchta</size=120%></b>
To hunt down <b><size=120%>rotten</size=120%></b> souls. To devour them. To worship…
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=140%><shiver>HER...</shiver></size=140%></b>

-> end_dialogue

=== end_dialogue
# speaker:<b><size=120%>Perchta</size=120%></b>
Off you go, <b>cub</b>. This house reeks of filth still.
# speaker:<b><size=120%>Perchta</size=120%></b>
Waste no time. <i>Be obedient</i>. My sister will come. <b>Soon…</b>

-> END