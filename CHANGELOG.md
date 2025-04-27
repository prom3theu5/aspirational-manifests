# Changelog

## [9.2.0](https://github.com/prom3theu5/aspirational-manifests/compare/v9.1.0...v9.2.0) (2025-04-27)


### 🚀 New Features

* added option to specify resources non-interactively ([#318](https://github.com/prom3theu5/aspirational-manifests/issues/318)) ([6ad05c1](https://github.com/prom3theu5/aspirational-manifests/commit/6ad05c192db3926c85b60dc066d0fa7e388751ef))
* project.v1 support. ([#314](https://github.com/prom3theu5/aspirational-manifests/issues/314)) ([c3924e2](https://github.com/prom3theu5/aspirational-manifests/commit/c3924e23d8dd4cad5d5cb3e4d243efb84d5afbd0))


### 🔥 Bug Fixes

* Added table output null check to handle headless services. ([#321](https://github.com/prom3theu5/aspirational-manifests/issues/321)) ([d785f6c](https://github.com/prom3theu5/aspirational-manifests/commit/d785f6c5f36660bccfc8f3f9909fb323a98ad63e))

## [9.1.0](https://github.com/prom3theu5/aspirational-manifests/compare/v9.0.4...v9.1.0) (2025-03-07)


### 🚀 New Features

* Container v1 support ([#309](https://github.com/prom3theu5/aspirational-manifests/issues/309)) ([adcf54c](https://github.com/prom3theu5/aspirational-manifests/commit/adcf54cad015ab40f21c5712abaf87b2809cd4a2))


### ⌨️ Code Refactoring

* Resource warning/error improvements ([#310](https://github.com/prom3theu5/aspirational-manifests/issues/310)) ([810a9a1](https://github.com/prom3theu5/aspirational-manifests/commit/810a9a1bf3939aa20e4d2ea3c3cd29eaf2d9d61e))

## [9.0.4](https://github.com/prom3theu5/aspirational-manifests/compare/v9.0.3...v9.0.4) (2025-02-28)


### 🔥 Bug Fixes

* docker compose port serialization. ComposeBuilder library doesn't automatically register the port type converter. ([#305](https://github.com/prom3theu5/aspirational-manifests/issues/305)) ([2eff1d4](https://github.com/prom3theu5/aspirational-manifests/commit/2eff1d4aa91c58158945f113bbc05e2957f6e9e7))

## [9.0.3](https://github.com/prom3theu5/aspirational-manifests/compare/v9.0.2...v9.0.3) (2025-02-28)


### 🔥 Bug Fixes

* set 'clusterIP' to 'None' in the case of headless service. ([#298](https://github.com/prom3theu5/aspirational-manifests/issues/298)) ([4fdc5b8](https://github.com/prom3theu5/aspirational-manifests/commit/4fdc5b808e8a0aac84c925b45b12bdd897da3191))

## [9.0.2](https://github.com/prom3theu5/aspirational-manifests/compare/v9.0.1...v9.0.2) (2025-02-16)


### ⚙️ Chores

* release 9.0.2 ([#293](https://github.com/prom3theu5/aspirational-manifests/issues/293)) ([7b0641a](https://github.com/prom3theu5/aspirational-manifests/commit/7b0641acd1223ce8d6b79de330f66ee86f9b1d5e))

## [9.0.1](https://github.com/prom3theu5/aspirational-manifests/compare/v9.0.0...v9.0.1) (2025-02-15)


### 🔥 Bug Fixes

* docs workflow remove deprecated actions ([#290](https://github.com/prom3theu5/aspirational-manifests/issues/290)) ([e9d0de5](https://github.com/prom3theu5/aspirational-manifests/commit/e9d0de5831c991449a01999334501cb94c5427c2))
* nullability on build args in container building for projects. Cannot call ToDictionary on null. ([#292](https://github.com/prom3theu5/aspirational-manifests/issues/292)) ([01e0ab7](https://github.com/prom3theu5/aspirational-manifests/commit/01e0ab7b6682d860fbea7e7e813def94e5277ebc))

## [9.0.0](https://github.com/prom3theu5/aspirational-manifests/compare/v9.0.0...v9.0.0) (2025-02-15)


### 🚀 New Features

* Add fieldManager, pretty, and fieldValidation options to resource creation ([#224](https://github.com/prom3theu5/aspirational-manifests/issues/224)) ([1192482](https://github.com/prom3theu5/aspirational-manifests/commit/1192482c22417cd4b990fd2d4858d081a3d843e7))
* **container:** Refactor handling of Docker push command ([#228](https://github.com/prom3theu5/aspirational-manifests/issues/228)) ([19f5dfb](https://github.com/prom3theu5/aspirational-manifests/commit/19f5dfbfb20cdc227f65757f0d64731ad487170b))


### 🔥 Bug Fixes

* **#275:** add support for the scheme property in endpoint bindings ([#276](https://github.com/prom3theu5/aspirational-manifests/issues/276)) ([d93fe56](https://github.com/prom3theu5/aspirational-manifests/commit/d93fe56b0ba86008768818efee84c8e9378499b3))
* 186 has secrets - make sure secret enabled state disables them in kube deploy data ([#187](https://github.com/prom3theu5/aspirational-manifests/issues/187)) ([305cd5a](https://github.com/prom3theu5/aspirational-manifests/commit/305cd5a3d1f32309f6498cdda125315bd47dd31e))
* version check error if not connection ([#284](https://github.com/prom3theu5/aspirational-manifests/issues/284)) ([bbf3617](https://github.com/prom3theu5/aspirational-manifests/commit/bbf3617f3da1d66db1fa790c1db3b743dce125bf))


### ⚙️ Chores

* release 9.0.0 ([#289](https://github.com/prom3theu5/aspirational-manifests/issues/289)) ([49f5f87](https://github.com/prom3theu5/aspirational-manifests/commit/49f5f879ef787c95cbdd3b3dca369d474fbd56be))


### 📦 CI Improvements

* Add and configure Release Please ([#287](https://github.com/prom3theu5/aspirational-manifests/issues/287)) ([3dab1a1](https://github.com/prom3theu5/aspirational-manifests/commit/3dab1a1d368c27c8402839008845ae55234202a4))
