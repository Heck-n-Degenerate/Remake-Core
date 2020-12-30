﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using StackExchange.Redis;
using SubterfugeRemakeService;
using SubterfugeServerConsole.Connections;
using SubterfugeServerConsole.Connections.Models;

namespace SubterfugeServerConsole
{
    public class SubterfugeServer : subterfugeService.subterfugeServiceBase
    {

        private RedisConnector redis;

        public SubterfugeServer(RedisConnector redis)
        {
            redis = redis;
        }
        
        public override Task<RoomDataResponse> GetRoomData(RoomDataRequest request, ServerCallContext context)
        {
            return base.GetRoomData(request, context);
        }

        public override Task<CreateRoomResponse> CreateNewRoom(CreateRoomRequest request, ServerCallContext context)
        {
            return base.CreateNewRoom(request, context);
        }

        public override Task<JoinRoomResponse> JoinRoom(JoinRoomRequest request, ServerCallContext context)
        {
            return base.JoinRoom(request, context);
        }

        public override Task<LeaveRoomResponse> LeaveRoom(LeaveRoomRequest request, ServerCallContext context)
        {
            return base.LeaveRoom(request, context);
        }

        public override Task<StartGameEarlyResponse> StartGameEarly(StartGameEarlyRequest request, ServerCallContext context)
        {
            return base.StartGameEarly(request, context);
        }

        public override Task<GetGameRoomEventsResponse> GetGameRoomEvents(GetGameRoomEventsRequest request, ServerCallContext context)
        {
            return base.GetGameRoomEvents(request, context);
        }

        public override Task<SubmitGameEventResponse> SubmitGameEvent(SubmitGameEventRequest request, ServerCallContext context)
        {
            return base.SubmitGameEvent(request, context);
        }

        public override Task<UpdateGameEventResponse> UpdateGameEvent(UpdateGameEventRequest request, ServerCallContext context)
        {
            return base.UpdateGameEvent(request, context);
        }

        public override Task<DeleteGameEventResponse> DeleteGameEvent(DeleteGameEventRequest request, ServerCallContext context)
        {
            return base.DeleteGameEvent(request, context);
        }

        public override Task<CreateMessageGroupResponse> CreateMessageGroup(CreateMessageGroupRequest request, ServerCallContext context)
        {
            return base.CreateMessageGroup(request, context);
        }

        public override Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
        {
            return base.SendMessage(request, context);
        }

        public override Task<GetMessageGroupsResponse> GetMessageGroups(GetMessageGroupsRequest request, ServerCallContext context)
        {
            return base.GetMessageGroups(request, context);
        }

        public override Task<BlockPlayerResponse> BlockPlayer(BlockPlayerRequest request, ServerCallContext context)
        {
            return base.BlockPlayer(request, context);
        }

        public override Task<UnblockPlayerResponse> UnblockPlayer(UnblockPlayerRequest request, ServerCallContext context)
        {
            return base.UnblockPlayer(request, context);
        }

        public override Task<ViewBlockedPlayersResponse> ViewBlockedPlayers(ViewBlockedPlayersRequest request, ServerCallContext context)
        {
            return base.ViewBlockedPlayers(request, context);
        }

        public override async Task<AuthorizationResponse> Login(AuthorizationRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Recieved Login Request");
            
            // Try to get a user
            RedisUserModel user = await RedisUserModel.getUser(request.Username);
            if (user == null || user.GetPassword() != request.Password)
            {
                if (user == null)
                {
                    Console.WriteLine($"null user");
                }
                Console.WriteLine($"Invalid login attempt");
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid Credentials."));
            }
            Console.WriteLine($"Login success");
            return new AuthorizationResponse {User = new User {Id = user.GetUserId(), Username = user.GetUsername()}};
        }

        public override async Task<AccountRegistrationResponse> RegisterAccount(AccountRegistrationRequest request,
            ServerCallContext context)
        {
            Console.WriteLine($"Recieved Register Request");
            RedisUserModel user = await RedisUserModel.getUser(request.Username);
            if (user == null)
            {
                
                Console.WriteLine($"Creating new user");
                // Create a new user model
                RedisUserModel model = RedisUserModel.newBuilder()
                    .setEmail(request.Email)
                    .setPassword(request.Password)
                    .setUsername(request.Username)
                    .Build();

                // Save the new user
                await model.saveUser();

                Console.WriteLine($"Created new user");
                
                return new AccountRegistrationResponse {User = new User {Id = model.GetUserId(), Username = model.GetUsername()}};    
            }
            
            Console.WriteLine($"User Exists.");
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Username already exists."));
        }
    }
}