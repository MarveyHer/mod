using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NeoModLoader.api;
using NeoModLoader.constants;
using NeoModLoader.ui;
using NeoModLoader.utils;
using RSG;
using Steamworks;
using Steamworks.Data;
using Steamworks.Ugc;
using UnityEngine;

namespace NeoModLoader.services;

internal class ModWorkshopServiceWindows : IPlatformSpecificModWorkshopService
{
	private static List<Item> subscribedItems = new List<Item>();

	private static Queue<Item> subscribedModsQueue = new Queue<Item>();

	public unsafe void UploadModLoader(string changelog)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		string text = SaveManager.generateWorkshopPath("NeoModLoader");
		string text2 = Path.Combine(text, "preview.png");
		if (Directory.Exists(text))
		{
			Directory.Delete(text, recursive: true);
		}
		Directory.CreateDirectory(text);
		File.Copy(Paths.NMLModPath, Path.Combine(text, "NeoModLoader.dll"));
		File.Copy(Paths.NMLModPath.Replace(".dll", ".pdb"), Path.Combine(text, "NeoModLoader.pdb"));
		using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("NeoModLoader.resources.logo.png");
		using FileStream fileStream = File.Create(text2);
		stream.Seek(0L, SeekOrigin.Begin);
		stream.CopyTo(fileStream);
		fileStream.Close();
		Editor val = new Editor(PublishedFileId.op_Implicit(3080294469uL));
		val = ((Editor)(ref val)).WithContent(text);
		val = ((Editor)(ref val)).WithTag("Mod Loader");
		val = ((Editor)(ref val)).WithPreviewFile(text2);
		Editor val2 = ((Editor)(ref val)).WithChangeLog(changelog);
		((Editor)(ref val2)).SubmitAsync((IProgress<float>)null).ContinueWith(delegate(Task<PublishResult> taskResult)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Invalid comparison between Unknown and I4
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (taskResult.Status != TaskStatus.RanToCompletion)
			{
				LogService.LogErrorConcurrent("!RanToCompletion");
			}
			else
			{
				PublishResult result = taskResult.Result;
				if (!((PublishResult)(ref result)).Success)
				{
					LogService.LogErrorConcurrent("!result.Success");
				}
				if (result.NeedsWorkshopAgreement)
				{
					PublishedFileId fileId = result.FileId;
					Application.OpenURL("steam://url/CommunityFilePage/" + ((object)(*(PublishedFileId*)(&fileId))/*cast due to constrained. prefix*/).ToString());
				}
				if ((int)result.Result != 1)
				{
					LogService.LogErrorConcurrent(((object)(*(Result*)(&result.Result))/*cast due to constrained. prefix*/).ToString());
				}
			}
		}, TaskScheduler.Default);
	}

	public unsafe Promise UploadMod(string name, string description, string previewImagePath, string workshopPath, string changelog, bool verified)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		Editor val = Editor.NewCommunityFile;
		val = ((Editor)(ref val)).WithTag(verified ? "Mod" : "Unverified Mod");
		val = ((Editor)(ref val)).WithTitle(name);
		val = ((Editor)(ref val)).WithDescription(description);
		val = ((Editor)(ref val)).WithPreviewFile(previewImagePath);
		val = ((Editor)(ref val)).WithContent(workshopPath);
		Editor val2 = ((Editor)(ref val)).WithChangeLog(changelog);
		Promise promise = new Promise();
		ModUploadingProgressWindow.UploadProgress uploadProgress = ModUploadingProgressWindow.ShowWindow();
		((Editor)(ref val2)).SubmitAsync((IProgress<float>)uploadProgress).ContinueWith(delegate(Task<PublishResult> taskResult)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Invalid comparison between Unknown and I4
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			if (taskResult.Status != TaskStatus.RanToCompletion)
			{
				promise.Reject(taskResult.Exception.GetBaseException());
			}
			else
			{
				PublishResult result = taskResult.Result;
				if (!((PublishResult)(ref result)).Success)
				{
					LogService.LogError("!result.Success");
				}
				if (result.NeedsWorkshopAgreement)
				{
					PublishedFileId fileId = result.FileId;
					Application.OpenURL("steam://url/CommunityFilePage/" + ((object)(*(PublishedFileId*)(&fileId))/*cast due to constrained. prefix*/).ToString());
				}
				if ((int)result.Result != 1)
				{
					promise.Reject(new Exception("Something went wrong: " + ((object)(*(Result*)(&result.Result))/*cast due to constrained. prefix*/).ToString()));
				}
				else
				{
					AbstractWindow<ModUploadingProgressWindow>.Instance.fileId = PublishedFileId.op_Implicit(result.FileId);
					promise.Resolve();
				}
			}
		}, TaskScheduler.Default);
		return promise;
	}

	public unsafe Promise EditMod(ulong fileID, string previewImagePath, string workshopPath, string changelog)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Promise promise = new Promise();
		Editor val = new Editor(PublishedFileId.op_Implicit(fileID));
		val = ((Editor)(ref val)).WithPreviewFile(previewImagePath);
		val = ((Editor)(ref val)).WithContent(workshopPath);
		Editor val2 = ((Editor)(ref val)).WithChangeLog(changelog);
		((Editor)(ref val2)).SubmitAsync((IProgress<float>)ModUploadingProgressWindow.ShowWindow()).ContinueWith(delegate(Task<PublishResult> taskResult)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Invalid comparison between Unknown and I4
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (taskResult.Status != TaskStatus.RanToCompletion)
			{
				promise.Reject(taskResult.Exception.GetBaseException());
			}
			else
			{
				PublishResult result = taskResult.Result;
				if (result.NeedsWorkshopAgreement)
				{
					LogService.LogWarning("Needs Workshop Agreement");
					PublishedFileId fileId = result.FileId;
					Application.OpenURL("steam://url/CommunityFilePage/" + ((object)(*(PublishedFileId*)(&fileId))/*cast due to constrained. prefix*/).ToString());
				}
				if ((int)result.Result != 1)
				{
					promise.Reject(new Exception(((object)(*(Result*)(&result.Result))/*cast due to constrained. prefix*/).ToString()));
				}
				else
				{
					promise.Resolve();
				}
			}
		}, TaskScheduler.FromCurrentSynchronizationContext());
		return promise;
	}

	public ModDeclare GetNextModFromWorkshopItem()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (subscribedModsQueue.Count == 0)
		{
			return null;
		}
		Item val = subscribedModsQueue.Dequeue();
		ModDeclare modDeclare = ModInfoUtils.recogMod(((Item)(ref val)).Directory);
		if (string.IsNullOrEmpty(modDeclare.RepoUrl))
		{
			string fileName = Path.GetFileName(((Item)(ref val)).Directory);
			modDeclare.SetRepoUrlToWorkshopPage(fileName);
		}
		return modDeclare;
	}

	public async void FindSubscribedMods()
	{
		foreach (Item item in await GetSubscribedItems())
		{
			subscribedModsQueue.Enqueue(item);
		}
	}

	private static async Task<List<Item>> GetSubscribedItems()
	{
		Query val = Query.ItemsReadyToUse;
		val = ((Query)(ref val)).WhereUserSubscribed(default(SteamId));
		Query q = ((Query)(ref val)).WithTag("Mod");
		q = ((Query)(ref q)).SortByCreationDateAsc();
		subscribedItems.Clear();
		int count = 1;
		int curr = 0;
		int page = 1;
		while (count > curr)
		{
			ResultPage? resultPage = await ((Query)(ref q)).GetPageAsync(page++);
			if (!resultPage.HasValue)
			{
				break;
			}
			count = resultPage.Value.TotalCount;
			curr += resultPage.Value.ResultCount;
			ResultPage value = resultPage.Value;
			foreach (Item entry2 in ((ResultPage)(ref value)).Entries)
			{
				Item entry = entry2;
				if (((Item)(ref entry)).IsInstalled && !((Item)(ref entry)).IsDownloadPending && !((Item)(ref entry)).IsDownloading)
				{
					if (!available(entry))
					{
						LogService.LogWarning("Incomplete mod " + ((Item)(ref entry)).Title + " found, skip");
					}
					else
					{
						subscribedItems.Add(entry);
					}
				}
				entry = default(Item);
			}
		}
		return subscribedItems;
		static bool available(Item item)
		{
			return true;
		}
	}
}
