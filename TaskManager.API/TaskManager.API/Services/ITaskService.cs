namespace TaskManager.API.Services
{
    public interface ITaskService
    {
        public Task<Models.TaskItem?> TaskInsert(DTOs.DTOs.CreateTaskDto dtotask, string userId); // Yeni görev oluşturur
        public Task<List<Models.TaskItem>?> TasktGetAll(); // Tüm görevleri getirir (test için)
        public Task<List<Models.TaskItem>?> TaskGetAllbyId(string id); // Kullanıcının tüm görevlerini getirir
        public Task<Models.TaskItem?> GetTaskById(string taskId, string userId); // Belirli bir görevi getirir
        public Task<bool> DeleteTask(string taskId, string userId); // Görevi siler
        public Task<Models.TaskItem?> UpdateTask(string taskId, string userId, DTOs.DTOs.UpdateTaskDto updateDto); // Görevi tamamen günceller
        public Task<Models.TaskItem?> PatchTask(string taskId, string userId, DTOs.DTOs.PatchTaskDto patchDto); // Görevi kısmi günceller
        public Task<List<Models.TaskItem>?> GetTasksByPeriod(string userId, string period); // Periyoda göre görevleri getirir
    }
}