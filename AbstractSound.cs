using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public abstract class AbstractSound {

	public AbstractSound () {
	}

	abstract public bool UpdateFrame (ref Frame frame);

}
