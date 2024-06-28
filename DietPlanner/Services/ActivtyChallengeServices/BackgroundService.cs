using Domain.Data;
using Domain.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


public class BackgroundService : IHostedService 
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public BackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ChallengeStatus, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        _timer = new Timer(UserChallengeStatus, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        _timer = new Timer(UserChallengesProgess, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void ChallengeStatus(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dietContext = scope.ServiceProvider.GetRequiredService<DietContext>();
            // Make changes to the DbContext and save them
            var challenges = dietContext.TblChallenges.ToList();
            var currentTime = DateTime.Now;

            foreach (var challenge in challenges)
            {
                if (challenge.StartDatetime <= currentTime && challenge.EndDatetime >= currentTime)
                {
                    challenge.ChallengeStatus = ChallengesRewardViewModel.ChallengeStatus.Ongoing.ToString();
                }
                else if (challenge.EndDatetime < currentTime)
                {
                    challenge.ChallengeStatus = ChallengesRewardViewModel.ChallengeStatus.Ended.ToString();
                }
                else if (challenge.StartDatetime > currentTime)
                {
                    challenge.ChallengeStatus = ChallengesRewardViewModel.ChallengeStatus.NotStarted.ToString();
                }
                dietContext.TblChallenges.Update(challenge);
            }
            dietContext.SaveChanges();
        }
    }

    private void UserChallengeStatus(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dietContext = scope.ServiceProvider.GetRequiredService<DietContext>();
            // Make changes to the DbContext and save them
            var challenges = dietContext.TblChallenges.ToList();
            var challengeslog = dietContext.TblChallengesRewardsLogs.ToList();
            var currentTime = DateTime.Now;

            foreach (var log in challengeslog)
            {
                var challenge = dietContext.TblChallenges.Where(challenge => challenge.ChallengeId == log.ChallengeId).FirstOrDefault();
                var newstatus = "null";
                if (log.Status == ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString())
                {
                    if (challenge.StartDatetime <= currentTime && challenge.EndDatetime >= currentTime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.OnGoing.ToString();
                    }
                    else if (currentTime > challenge.EndDatetime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.Completed.ToString();
                    }
                    else if (currentTime < challenge.StartDatetime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString();
                    }
                }
                else if (log.Status == ChallengesRewardViewModel.UserChallengeStatus.OnGoing.ToString())
                {
                    if((challenge.StartDatetime <= currentTime && challenge.EndDatetime >= currentTime))
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.OnGoing.ToString();
                    }
                    else if (currentTime > challenge.EndDatetime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.Completed.ToString();
                    }
                    else if (currentTime < challenge.StartDatetime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.Registered.ToString();
                    }
                }
                else if (log.Status == ChallengesRewardViewModel.UserChallengeStatus.NotRegistered.ToString())
                {
                    if (challenge.StartDatetime <= currentTime && challenge.EndDatetime >= currentTime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.NotRegistered.ToString();
                    }
                    else if (currentTime > challenge.EndDatetime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.NotCompleted.ToString();
                    }
                    else if (currentTime < challenge.StartDatetime)
                    {
                        newstatus = ChallengesRewardViewModel.UserChallengeStatus.NotRegistered.ToString();
                    }
                }
                if (log.Status != newstatus && newstatus != "null")
                {
                    log.Status = newstatus;
                    log.StatusDatetime = DateTime.Now;
                }
                dietContext.TblChallengesRewardsLogs.Update(log);
            }
            dietContext.SaveChanges();
        }
    }

    private void UserChallengesProgess(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dietContext = scope.ServiceProvider.GetRequiredService<DietContext>();
            var challengeslog = dietContext.TblChallengesRewardsLogs.ToList();
            var reward = dietContext.TblRewards.ToList();
            var activityDetails = dietContext.TblActivityTrackings.ToList();
            var currentTime = DateTime.Now;

            foreach (var log in challengeslog)
            {
                var challenge = dietContext.TblChallenges.Where(challenge => challenge.ChallengeId == log.ChallengeId).FirstOrDefault();
                var userActivity = dietContext.TblActivityTrackings
                    .Where(activity =>
                        activity.ProfileId == log.ProfileId && 
                        activity.ActivityEndDatetime >= challenge.StartDatetime &&
                        activity.ActivityEndDatetime <= challenge.EndDatetime).ToList();

               
                int Totalcalorie=0;
                foreach (var activity in userActivity)
                {
                    Totalcalorie += activity.CalorieBurned;
                }
                if(Totalcalorie >= int.Parse(challenge.ChallengeGoals))
                {
                    log.RewardId = dietContext.TblRewards.Where(reward => reward.ChallengeId == challenge.ChallengeId).Select(reward=>reward.RewardId).FirstOrDefault();
                }
                double progress = 0;

                progress = Math.Round(((double)Totalcalorie / double.Parse(challenge.ChallengeGoals)) * 100, 3);

                log.ChallengeProgress = progress;
                dietContext.TblChallengesRewardsLogs.Update(log);

            }
            dietContext.SaveChanges();
        }
    }
    
}