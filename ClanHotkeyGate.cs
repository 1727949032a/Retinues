using System;
using TaleWorlds.InputSystem;

// Token: 0x02000010 RID: 16
public static class ClanHotkeyGate
{
	// Token: 0x06000029 RID: 41 RVA: 0x00002187 File Offset: 0x00000387
	public static bool Matches(InputKey key)
	{
		return ClanHotkeyGate.Active && key == InputKey.L && (!ClanHotkeyGate.RequireShift || (!Input.IsKeyDown(InputKey.LeftShift) && !Input.IsKeyDown(InputKey.RightShift)));
	}

	// Token: 0x04000005 RID: 5
	public static volatile bool Active;

	// Token: 0x04000006 RID: 6
	public static bool RequireShift = true;
}
