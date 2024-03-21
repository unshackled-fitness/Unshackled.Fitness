﻿namespace Unshackled.Fitness.Core;

public class RowContext<T> where T : new()
{
	public required T Item { get; set; }
	public int RowIndex { get; set; }
}
