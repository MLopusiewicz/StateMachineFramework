using NUnit.Framework;
using StateMachineFramework.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class ParametersTests { 
    public GameObject go;
    ParameterController GetController() {
        return new ParameterController(new List<IParameter> {
            new TriggerParameter("Trigger", false),
            new BoolParameter("Bool", false),
            new FloatParameter("Float", 0),
            new IntParameter("Int", 0),
        });
    }

    [Test]
    public void LUT() {
        ParameterController controller = GetController();
        Assert.IsTrue(controller.GetTrigger("Trigger") == false);
        Assert.IsTrue(controller.GetBool("Bool") == false);
        Assert.IsTrue(controller.GetFloat("Float") == 0);
        Assert.IsTrue(controller.GetInt("Int") == 0);

    }

    [Test]
    public void SetTrigger() {
        ParameterController controller = GetController();
        controller.SetTrigger("Trigger");
        Assert.IsTrue(controller.GetTrigger("Trigger") == true);
    }

    [Test]
    public void SetBool() {
        ParameterController controller = GetController();
        controller.SetBool("Bool", true);
        Assert.IsTrue(controller.GetBool("Bool") == true);
    }

    [Test]
    public void SetFloat() {
        ParameterController controller = GetController();
        controller.SetFloat("Float", 10);
        Assert.IsTrue(controller.GetFloat("Float") == 10);
    }
     
    [Test]
    public void SetInt() {
        ParameterController controller = GetController();
        controller.SetInt("Int", 10);
        Assert.IsTrue(controller.GetInt("Int") == 10);
    }

}
