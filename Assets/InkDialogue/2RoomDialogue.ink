VAR girl = "Sprites/Perhta2"
VAR perchta = ""

=== Perchta ===
# speaker:<b><size=120%>Perchta</size=120%></b>
mmm… I see, <b>little cub</b> can fight for its life.
# speaker:<b><size=120%>Perchta</size=120%></b>
I smell no blood and see no dust, yet I wonder…
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
There’s so much <shiver><b>meat</shiver></b> left… Yet it is not me who hunted her down.

-> end_dialogue

=== want ===
# speaker:<b><size=120%>Perchta</size=120%></b>
To hunt down <b>rotten</b> souls. To devour them. To worship…
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=140%><shiver>HER...</shiver></size=140%></b>

-> end_dialogue

=== end_dialogue

-> END