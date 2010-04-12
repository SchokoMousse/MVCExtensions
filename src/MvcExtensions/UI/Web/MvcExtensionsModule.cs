﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Spark.Web.Mvc;
using MvcExtensions.Model;
using MvcExtensions.Services;
using MvcExtensions.Services.Impl;
using Castle.MicroKernel.Registration;
using MvcExtensions.Services.Impl.FluentNHibernate;
using MvcExtensions.UI.Web.ModelBinders;
using MvcExtensions.UI.Web.Controller;
using Spark;

namespace MvcExtensions.UI.Web
{
    public class MvcExtensionsModule : IModule
    {
        MvcContainer _Container = null;
        public MvcContainer Container
        {
            get
            {
                _Container = _Container ?? new MvcContainer();
                return _Container;
            }
            set { _Container = value; }
        }
        public MvcExtensionsModule()
        {
        }

        public void RegisterClassDerivedFromMyTextInModelBinder(Type t)
        {
            var binders = System.Web.Mvc.ModelBinders.Binders;
            var x = typeof(MyTextModelBinder<>).MakeGenericType(t);
            binders.Add(t, (IModelBinder)(Activator.CreateInstance(x)));
        }

        public void Register(Database database) 
        {
            var settings = new Spark.SparkSettings()
                .AddAssembly(this.GetType().Assembly)
                .SetAutomaticEncoding(true)
                .SetDebug(true)
                .AddNamespace(typeof(MvcExtensions.UI.Web.Helpers.HTMLHelperExtensions).Namespace)
                .AddNamespace(typeof(MvcExtensions.UI.ViewModel.VMActionLink).Namespace);
            var spv = new SparkViewFactory(settings);
            var path = "MvcExtensions.UI.Web.Views";
            spv.AddEmbeddedResources(this.GetType().Assembly,path);
            ViewEngines.Engines.Add(spv);

            var fact = new MvcContainerControllerFactory(Container);
            ControllerBuilder.Current.SetControllerFactory(fact);

            var cont = Container;
            cont.AddFacility<Castle.Facilities.FactorySupport.FactorySupportFacility>();

            foreach (var t in this.GetType().Assembly.GetExportedTypes()
                .Where(tp => tp.Namespace == typeof(ShortText).Namespace 
                        && !tp.IsAbstract 
                        && typeof(MyText).IsAssignableFrom(tp)))
            {
                RegisterClassDerivedFromMyTextInModelBinder(t);
            }

            cont.Register(
                Component.For<IRepository>().ImplementedBy<Repository>().LifeStyle.PerWebRequest,
                Component.For<Database>().Instance(database),
                Component.For<IUnitOfWork>().LifeStyle.PerWebRequest
                    .UsingFactoryMethod(x => x.Resolve<Database>().CurrentUnitOfWork)
                );
        }
    }
}
