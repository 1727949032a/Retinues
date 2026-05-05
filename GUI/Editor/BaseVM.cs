using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace Retinues.GUI.Editor
{
	// Token: 0x0200006E RID: 110
	public abstract class BaseVM : ViewModel
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000EC47 File Offset: 0x0000CE47
		// (set) Token: 0x06000216 RID: 534 RVA: 0x0000EC4F File Offset: 0x0000CE4F
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (this._isVisible == value)
				{
					return;
				}
				this._isVisible = value;
				base.OnPropertyChanged("IsVisible");
				if (this._isVisible)
				{
					this.FlushIfPending();
				}
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000EC7B File Offset: 0x0000CE7B
		public virtual void Show()
		{
			this.IsVisible = true;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000EC84 File Offset: 0x0000CE84
		public virtual void Hide()
		{
			this.IsVisible = false;
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000219 RID: 537
		protected abstract Dictionary<UIEvent, string[]> EventMap { get; }

		// Token: 0x0600021A RID: 538 RVA: 0x0000EC8D File Offset: 0x0000CE8D
		protected BaseVM() : this(true)
		{
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000EC96 File Offset: 0x0000CE96
		protected BaseVM(bool autoRegister = true)
		{
			if (autoRegister)
			{
				EventManager.Register(this);
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000ECB4 File Offset: 0x0000CEB4
		~BaseVM()
		{
			EventManager.Unregister(this);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000ECE0 File Offset: 0x0000CEE0
		internal void __BeginPulse()
		{
			this._inPulse = true;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000ECE9 File Offset: 0x0000CEE9
		internal void __EndPulse()
		{
			this._inPulse = false;
			if (this.IsVisible)
			{
				this.FlushPending();
				return;
			}
			this._queuedWhileHidden = true;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000ED08 File Offset: 0x0000CF08
		internal void __OnGlobalPulse(UIEvent e)
		{
			string[] array;
			if (this.EventMap.TryGetValue(e, out array) && array != null)
			{
				foreach (string text in array)
				{
					if (!string.IsNullOrEmpty(text))
					{
						this._pendingProps.Add(text);
					}
				}
			}
			if (e == UIEvent.Faction)
			{
				this.OnFactionChange();
			}
			else if (e == UIEvent.Troop)
			{
				this.OnTroopChange();
			}
			else if (e == UIEvent.Equipment)
			{
				this.OnEquipmentChange();
			}
			else if (e == UIEvent.Slot)
			{
				this.OnSlotChange();
			}
			else if (e == UIEvent.Equip)
			{
				this.OnEquipChange();
			}
			else if (e == UIEvent.Conversion)
			{
				this.OnConversionChange();
			}
			if (!this.IsVisible)
			{
				this._queuedWhileHidden = true;
				return;
			}
			if (this._inPulse || !this.IsVisible)
			{
				return;
			}
			this.FlushPending();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000EDBB File Offset: 0x0000CFBB
		private void FlushIfPending()
		{
			if (this._queuedWhileHidden && !this._inPulse && this.IsVisible)
			{
				this.FlushPending();
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000EDDC File Offset: 0x0000CFDC
		protected void FlushPending()
		{
			if (this._pendingProps.Count == 0)
			{
				this._queuedWhileHidden = false;
				return;
			}
			foreach (string propertyName in this._pendingProps)
			{
				base.OnPropertyChanged(propertyName);
			}
			this._pendingProps.Clear();
			this._queuedWhileHidden = false;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000EE58 File Offset: 0x0000D058
		protected virtual void OnFactionChange()
		{
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000EE5A File Offset: 0x0000D05A
		protected virtual void OnTroopChange()
		{
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000EE5C File Offset: 0x0000D05C
		protected virtual void OnEquipmentChange()
		{
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000EE5E File Offset: 0x0000D05E
		protected virtual void OnSlotChange()
		{
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000EE60 File Offset: 0x0000D060
		protected virtual void OnEquipChange()
		{
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000EE62 File Offset: 0x0000D062
		protected virtual void OnConversionChange()
		{
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000EE64 File Offset: 0x0000D064
		protected static int BatchInput(bool capped = true)
		{
			if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl))
			{
				if (!capped)
				{
					return 1000;
				}
				return 5;
			}
			else
			{
				if (Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift))
				{
					return 5;
				}
				return 1;
			}
		}

		// Token: 0x040000AE RID: 174
		private bool _isVisible;

		// Token: 0x040000AF RID: 175
		private readonly HashSet<string> _pendingProps = new HashSet<string>();

		// Token: 0x040000B0 RID: 176
		private bool _queuedWhileHidden;

		// Token: 0x040000B1 RID: 177
		private bool _inPulse;
	}
}
