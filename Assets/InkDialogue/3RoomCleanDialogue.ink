VAR girl = "Sprites/Perhta4"
VAR perchta = ""

=== Perchta ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>You’ve cleaned…</size=160%></b>
<color="black">.</color="black">
    + [<i><shiver>nod...</shiver></i>]
        -> first_choice
    + [<i>Will you leave me now?</i>]
        -> first_choice

=== first_choice ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>You’re different… not like your mother.</size=160%></b>
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%><shiver>Interesting…</shiver></size=160%></b>
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>We will spare your life, child…</size=160%></b>
<color="black">.</color="black">
    + [<i>leave <b>my</b> house…</i>]
        -> my
    + [<i>leave <b>me<b> alone!</i>]
        -> me

=== my ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>Resilient soul…</size=160%></b>
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>We will come back. Keep this house clean.</size=160%></b>

-> END

=== me ===
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>Still scared and shattered…</size=160%></b>
# speaker:<b><size=120%>Perchta</size=120%></b>
<b><size=160%>A fine addition…</size=160%></b>

-> END